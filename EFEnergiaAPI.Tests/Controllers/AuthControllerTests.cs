using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EFEnergiaAPI.Controllers;
using EFEnergiaAPI.Data;
using Api.Models;
using System.Collections.Generic;

namespace EFEnergiaAPI.Tests.Controllers;

public class AuthControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        // Setup mock configuration
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("YourSuperSecretKeyForJWTTokenGenerationThatMustBeAtLeast32Characters");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("EFEnergiaAPI");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("EFEnergiaAPI");
        _mockConfiguration.Setup(c => c["Jwt:ExpirationMinutes"]).Returns("60");

        // Setup real DbContext with in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _controller = new AuthController(_context, _mockConfiguration.Object);
    }

    [Fact]
    public async Task SeedAdmin_ReturnsHttpStatusCode200_WhenNoUsersExist()
    {
        // Arrange - Database is empty (no users exist)

        // Act
        var result = await _controller.SeedAdmin();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        
        // Verify that a user was created
        var userExists = await _context.Users.AnyAsync(u => u.Username == "admin");
        Assert.True(userExists);
    }
}

// Helper class for async enumerable
internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }
}

