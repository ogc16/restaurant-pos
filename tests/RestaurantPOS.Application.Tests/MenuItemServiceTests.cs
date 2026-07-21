using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using RestaurantPOS.Application.Services;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Tests;

public class MenuItemServiceTests
{
    private readonly Mock<IMenuItemRepository> _repoMock = new();
    private readonly Mock<ILogger<MenuItemService>> _loggerMock = new();
    private readonly MenuItemService _sut;

    public MenuItemServiceTests()
    {
        _sut = new MenuItemService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsAllItems()
    {
        var items = new List<MenuItem>
        {
            new() { Id = 1, Name = "Burger", Price = 12.99m, Category = MenuItemCategory.Main, IsAvailable = true },
            new() { Id = 2, Name = "Fries", Price = 4.99m, Category = MenuItemCategory.Side, IsAvailable = true }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

        var result = await _sut.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].Name.Should().Be("Burger");
        result.Data[1].Name.Should().Be("Fries");
    }

    [Fact]
    public async Task GetById_ExistingItem_ReturnsItem()
    {
        var item = new MenuItem { Id = 1, Name = "Burger", Description = "Tasty", Price = 12.99m, Category = MenuItemCategory.Main, IsAvailable = true, PrepTimeMinutes = 10 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Burger");
        result.Data.Price.Should().Be(12.99m);
    }

    [Fact]
    public async Task GetById_NonExistingItem_ReturnsFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((MenuItem?)null);

        var result = await _sut.GetByIdAsync(99);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Menu item not found");
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedItem()
    {
        var request = new CreateMenuItemRequest("Pizza", "Wood-fired", 15.99m, MenuItemCategory.Main, null, 15, false);
        var created = new MenuItem { Id = 1, Name = "Pizza", Description = "Wood-fired", Price = 15.99m, Category = MenuItemCategory.Main, PrepTimeMinutes = 15 };
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<MenuItem>())).ReturnsAsync(created);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Pizza");
        result.Data.Price.Should().Be(15.99m);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<MenuItem>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ExistingItem_ReturnsSuccess()
    {
        var item = new MenuItem { Id = 1, Name = "Burger" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.DeleteAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task ToggleAvailability_TogglesFlag()
    {
        var item = new MenuItem { Id = 1, Name = "Burger", IsAvailable = true };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.ToggleAvailabilityAsync(1);

        result.IsSuccess.Should().BeTrue();
        item.IsAvailable.Should().BeFalse();
        _repoMock.Verify(r => r.UpdateAsync(item), Times.Once);
    }
}
