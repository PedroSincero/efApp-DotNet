using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Controllers;
using EFEnergiaAPI.Services.Equipamento;
using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Equipamento;

namespace EFEnergiaAPI.Tests.Controllers;

public class EquipamentosControllerTests
{
    private readonly Mock<IEquipamentoService> _mockService;
    private readonly EquipamentosController _controller;

    public EquipamentosControllerTests()
    {
        _mockService = new Mock<IEquipamentoService>();
        _controller = new EquipamentosController(_mockService.Object);
    }

    [Fact]
    public async Task GetEquipamentos_ReturnsHttpStatusCode200()
    {
        // Arrange
        var expectedResult = new PagedResultViewModel<EquipamentoViewModel>
        {
            TotalItems = 2,
            Page = 1,
            PageSize = 10,
            Items = new List<EquipamentoViewModel>
            {
                new EquipamentoViewModel { Id = 1, Nome = "Equipamento 1", SetorId = 1 },
                new EquipamentoViewModel { Id = 2, Nome = "Equipamento 2", SetorId = 1 }
            }
        };

        _mockService.Setup(s => s.GetEquipamentosAsync(1, 10))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetEquipamentos(1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockService.Verify(s => s.GetEquipamentosAsync(1, 10), Times.Once);
    }
}

