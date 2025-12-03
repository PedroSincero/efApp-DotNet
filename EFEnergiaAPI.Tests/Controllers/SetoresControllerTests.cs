using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EFEnergiaAPI.Controllers;
using EFEnergiaAPI.Services.Setor;
using EFEnergiaAPI.ViewModels;
using EFEnergiaAPI.ViewModels.Setor;

namespace EFEnergiaAPI.Tests.Controllers;

public class SetoresControllerTests
{
    private readonly Mock<ISetorService> _mockService;
    private readonly SetoresController _controller;

    public SetoresControllerTests()
    {
        _mockService = new Mock<ISetorService>();
        _controller = new SetoresController(_mockService.Object);
    }

    [Fact]
    public async Task GetSetores_ReturnsHttpStatusCode200()
    {
        // Arrange
        var expectedResult = new PagedResultViewModel<SetorViewModel>
        {
            TotalItems = 2,
            Page = 1,
            PageSize = 10,
            Items = new List<SetorViewModel>
            {
                new SetorViewModel { Id = 1, Nome = "Setor 1" },
                new SetorViewModel { Id = 2, Nome = "Setor 2" }
            }
        };

        _mockService.Setup(s => s.GetSetoresAsync(1, 10))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetSetores(1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockService.Verify(s => s.GetSetoresAsync(1, 10), Times.Once);
    }
}

