using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Services;

namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly IHealthCheckService _healthCheckService;

    public HealthCheckController(IHealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Verifica o status da aplicação
    /// </summary>
    /// <returns>Status da aplicação e informações do sistema</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var healthStatus = await _healthCheckService.CheckHealthAsync();
        return Ok(healthStatus);
    }
}

