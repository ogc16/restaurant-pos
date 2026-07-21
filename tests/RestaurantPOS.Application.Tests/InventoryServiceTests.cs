using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using RestaurantPOS.Application.Services;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Tests;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _repoMock = new();
    private readonly Mock<ILogger<InventoryService>> _loggerMock = new();
    private readonly InventoryService _sut;

    public InventoryServiceTests()
    {
        _sut = new InventoryService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsItems()
    {
        var items = new List<InventoryItem>
        {
            new() { Id = 1, Name = "Tomatoes", Quantity = 50, Unit = "kg", CostPerUnit = 2.5m },
            new() { Id = 2, Name = "Cheese", Quantity = 20, Unit = "kg", CostPerUnit = 8.0m }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

        var result = await _sut.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].Name.Should().Be("Tomatoes");
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsItem()
    {
        var request = new CreateInventoryItemRequest("Flour", "All-purpose", 100, "kg", 10, "ACME", null, 1.5m);
        var created = new InventoryItem { Id = 1, Name = "Flour", Description = "All-purpose", Quantity = 100, Unit = "kg", CostPerUnit = 1.5m };
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<InventoryItem>())).ReturnsAsync(created);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Flour");
        result.Data.Quantity.Should().Be(100);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<InventoryItem>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ExistingItem_ReturnsSuccess()
    {
        var item = new InventoryItem { Id = 1, Name = "Tomatoes" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.DeleteAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Restock_ValidRequest_IncreasesQuantity()
    {
        var item = new InventoryItem { Id = 1, Name = "Tomatoes", Quantity = 50, CostPerUnit = 2.5m };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        _repoMock.Setup(r => r.LogRestockAsync(It.IsAny<RestockLog>())).ReturnsAsync(new RestockLog());

        var request = new RestockRequest(1, 30, 2.0m, "ACME", "Weekly restock", "Admin");

        var result = await _sut.RestockAsync(request);

        result.IsSuccess.Should().BeTrue();
        item.Quantity.Should().Be(80);
        item.CostPerUnit.Should().Be(2.0m);
        _repoMock.Verify(r => r.UpdateAsync(item), Times.Once);
        _repoMock.Verify(r => r.LogRestockAsync(It.IsAny<RestockLog>()), Times.Once);
    }
}
