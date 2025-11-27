using EFEnergiaAPI.Models;

namespace EFEnergiaAPI.Services;

public interface IHealthCheckService
{
    Task<HealthCheckResponse> CheckHealthAsync();
}

