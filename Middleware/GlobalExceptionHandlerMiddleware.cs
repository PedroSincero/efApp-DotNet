using System.Net;
using System.Text.Json;
using EFEnergiaAPI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EFEnergiaAPI.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Ocorreu um erro interno no servidor.";
        var details = exception.Message;

        // Tratamento específico para diferentes tipos de exceções
        switch (exception)
        {
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                details = null;
                break;

            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                details = null;
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "Acesso não autorizado.";
                details = exception.Message;
                break;

            case KeyNotFoundException:
            case EntityNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                details = null;
                break;

            case DbUpdateConcurrencyException:
                statusCode = HttpStatusCode.Conflict;
                message = "O registro foi modificado por outro usuário. Por favor, atualize e tente novamente.";
                details = exception.Message;
                break;

            case DbUpdateException dbEx:
                statusCode = HttpStatusCode.BadRequest;
                message = "Erro ao atualizar o banco de dados.";
                details = dbEx.InnerException?.Message ?? dbEx.Message;
                break;
        }

        var response = new ErrorResponseViewModel
        {
            Message = message,
            StatusCode = (int)statusCode,
            Timestamp = DateTime.UtcNow,
            Details = details
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(json);
    }
}

// Classe auxiliar para exceções de entidade não encontrada
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message) : base(message) { }
}

