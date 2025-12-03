using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Controllers;
using EFEnergiaAPI.Services.Alerta;
using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Alerta;

namespace EFEnergiaAPI.Tests.Controllers;

public class AlertasControllerTests
{
    private readonly Mock<IAlertaService> _mockService;
    private readonly AlertasController _controller;

    public AlertasControllerTests()
    {
        _mockService = new Mock<IAlertaService>();
        _controller = new AlertasController(_mockService.Object);
    }

    [Fact]
    public async Task GetAlertas_ReturnsHttpStatusCode200()
    {
        // Arrange
        var expectedResult = new PagedResultViewModel<AlertaViewModel>
        {
            TotalItems = 2,
            Page = 1,
            PageSize = 10,
            Items = new List<AlertaViewModel>
            {
                new AlertaViewModel { Id = 1, EquipamentoId = 1, Mensagem = "Alerta 1", DataCriacao = DateTime.UtcNow, Resolvido = false },
                new AlertaViewModel { Id = 2, EquipamentoId = 1, Mensagem = "Alerta 2", DataCriacao = DateTime.UtcNow, Resolvido = true }
            }
        };

        _mockService.Setup(s => s.GetAlertasAsync(1, 10))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetAlertas(1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockService.Verify(s => s.GetAlertasAsync(1, 10), Times.Once);
    }
}

