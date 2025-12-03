using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Controllers;
using EFEnergiaAPI.Services;
using EFEnergiaAPI.Models;

namespace EFEnergiaAPI.Tests.Controllers;

public class HealthCheckControllerTests
{
    private readonly Mock<IHealthCheckService> _mockService;
    private readonly HealthCheckController _controller;

    public HealthCheckControllerTests()
    {
        _mockService = new Mock<IHealthCheckService>();
        _controller = new HealthCheckController(_mockService.Object);
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var expectedResponse = new HealthCheckResponse
        {
            Status = "UP",
            Database = "Connected",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        };

        _mockService.Setup(s => s.CheckHealthAsync())
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockService.Verify(s => s.CheckHealthAsync(), Times.Once);
    }
}

