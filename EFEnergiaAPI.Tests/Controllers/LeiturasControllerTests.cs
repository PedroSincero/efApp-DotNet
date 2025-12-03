using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Controllers;
using EFEnergiaAPI.Services.Leitura;
using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Leitura;

namespace EFEnergiaAPI.Tests.Controllers;

public class LeiturasControllerTests
{
    private readonly Mock<ILeituraService> _mockService;
    private readonly LeiturasController _controller;

    public LeiturasControllerTests()
    {
        _mockService = new Mock<ILeituraService>();
        _controller = new LeiturasController(_mockService.Object);
    }

    [Fact]
    public async Task GetLeituras_ReturnsHttpStatusCode200()
    {
        // Arrange
        var expectedResult = new PagedResultViewModel<LeituraViewModel>
        {
            TotalItems = 2,
            Page = 1,
            PageSize = 10,
            Items = new List<LeituraViewModel>
            {
                new LeituraViewModel { Id = 1, EquipamentoId = 1, Temperatura = 25.5m, DataRegistro = DateTime.UtcNow },
                new LeituraViewModel { Id = 2, EquipamentoId = 1, Temperatura = 26.0m, DataRegistro = DateTime.UtcNow }
            }
        };

        _mockService.Setup(s => s.GetLeiturasAsync(1, 10))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetLeituras(1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockService.Verify(s => s.GetLeiturasAsync(1, 10), Times.Once);
    }
}

