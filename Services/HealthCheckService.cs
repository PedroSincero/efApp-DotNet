using EFEnergiaAPI.Data;
using EFEnergiaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EFEnergiaAPI.Services;

public class HealthCheckService : IHealthCheckService
{
    private readonly ApplicationDbContext _context;

    public HealthCheckService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResponse> CheckHealthAsync()
    {
        var response = new HealthCheckResponse
        {
            Status = "UP",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        };

        try
        {
            // Verifica conexão com banco de dados
            if (_context != null)
            {
                var canConnect = await _context.Database.CanConnectAsync();
                response.Database = canConnect ? "Connected" : "Disconnected";
            }
            else
            {
                response.Database = "Not configured";
            }
        }
        catch (Exception ex)
        {
            response.Database = $"Error: {ex.Message}";
            response.Status = "DEGRADED";
        }

        return response;
    }
}

