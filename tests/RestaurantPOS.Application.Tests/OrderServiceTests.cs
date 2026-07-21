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

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<IMenuItemRepository> _menuRepoMock = new();
    private readonly Mock<ITableRepository> _tableRepoMock = new();
    private readonly Mock<IInventoryRepository> _inventoryRepoMock = new();
    private readonly Mock<IPromotionRepository> _promoRepoMock = new();
    private readonly Mock<IPaymentRepository> _paymentRepoMock = new();
    private readonly Mock<ILogger<OrderService>> _loggerMock = new();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(
            _orderRepoMock.Object,
            _menuRepoMock.Object,
            _tableRepoMock.Object,
            _inventoryRepoMock.Object,
            _promoRepoMock.Object,
            _paymentRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOrders()
    {
        var orders = new List<Order>
        {
            new() { Id = 1, TableId = 1, Status = OrderStatus.Pending, SubTotal = 20m },
            new() { Id = 2, TableId = 2, Status = OrderStatus.Completed, SubTotal = 35m }
        };
        _orderRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

        var result = await _sut.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_ExistingOrder_ReturnsOrder()
    {
        var order = new Order
        {
            Id = 1,
            TableId = 1,
            Table = new Table { TableNumber = "T1" },
            Status = OrderStatus.Pending,
            SubTotal = 20m,
            Items = new List<OrderItem>()
        };
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        var result = await _sut.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_NonExistingOrder_ReturnsFailure()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Order?)null);

        var result = await _sut.GetByIdAsync(99);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Order not found");
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsOrder()
    {
        var table = new Table { Id = 1, TableNumber = "T1", Status = TableStatus.Available };
        var menuItem = new MenuItem { Id = 1, Name = "Burger", Price = 10m, IsAvailable = true, MenuItemInventories = new List<MenuItemInventory>() };
        _tableRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(table);
        _menuRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(menuItem);

        var createdOrder = new Order
        {
            Id = 1,
            TableId = 1,
            Status = OrderStatus.Pending,
            SubTotal = 10m,
            TaxAmount = 0.8m,
            Items = new List<OrderItem>
            {
                new() { Id = 1, MenuItemId = 1, Quantity = 1, UnitPrice = 10m }
            }
        };
        _orderRepoMock.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync(createdOrder);

        var request = new CreateOrderRequest(1, 1, null, "Waiter Bob", null,
            new List<CreateOrderItemRequest> { new(1, 1, null) }, null);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.SubTotal.Should().Be(10m);
        table.Status.Should().Be(TableStatus.Occupied);
        _tableRepoMock.Verify(r => r.UpdateAsync(table), Times.Once);
    }

    [Fact]
    public async Task Create_TableNotFound_ReturnsFailure()
    {
        _tableRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Table?)null);

        var request = new CreateOrderRequest(99, 1, null, null, null,
            new List<CreateOrderItemRequest>(), null);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Table not found");
    }

    [Fact]
    public async Task Create_MenuItemNotFound_ReturnsFailure()
    {
        var table = new Table { Id = 1, TableNumber = "T1", Status = TableStatus.Available };
        _tableRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(table);
        _menuRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((MenuItem?)null);

        var request = new CreateOrderRequest(1, 1, null, null, null,
            new List<CreateOrderItemRequest> { new(99, 1, null) }, null);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Menu item 99 not found");
    }

    [Fact]
    public async Task UpdateStatus_CompletedOrder_ReleasesTable()
    {
        var table = new Table { Id = 1, TableNumber = "T1", Status = TableStatus.Occupied };
        var order = new Order
        {
            Id = 1,
            TableId = 1,
            Table = table,
            Status = OrderStatus.Served,
            SubTotal = 20m,
            Items = new List<OrderItem>()
        };
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _tableRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(table);

        var request = new UpdateOrderRequest(OrderStatus.Completed, null);

        var result = await _sut.UpdateStatusAsync(1, request);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Completed);
        table.Status.Should().Be(TableStatus.Available);
        _tableRepoMock.Verify(r => r.UpdateAsync(table), Times.Once);
    }

    [Fact]
    public async Task ApplyPromotion_ValidCode_AppliesDiscount()
    {
        var order = new Order
        {
            Id = 1,
            TableId = 1,
            SubTotal = 100m,
            DiscountAmount = 0,
            Items = new List<OrderItem>()
        };
        var promo = new Promotion
        {
            Id = 1,
            Code = "SAVE10",
            IsPercentBased = true,
            DiscountPercent = 10,
            CurrentUses = 0,
            MaxUses = 100,
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidUntil = DateTime.UtcNow.AddDays(30)
        };
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _promoRepoMock.Setup(r => r.GetByCodeAsync("SAVE10")).ReturnsAsync(promo);

        var request = new ApplyPromotionRequest("SAVE10");

        var result = await _sut.ApplyPromotionAsync(1, request);

        result.IsSuccess.Should().BeTrue();
        order.DiscountAmount.Should().Be(10m);
        promo.CurrentUses.Should().Be(1);
    }

    [Fact]
    public async Task ApplyPromotion_InvalidCode_ReturnsFailure()
    {
        var order = new Order { Id = 1, TableId = 1, SubTotal = 100m, DiscountAmount = 0, Items = new List<OrderItem>() };
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _promoRepoMock.Setup(r => r.GetByCodeAsync("INVALID")).ReturnsAsync((Promotion?)null);

        var result = await _sut.ApplyPromotionAsync(1, new ApplyPromotionRequest("INVALID"));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Promotion not found");
    }

    [Fact]
    public async Task ApplyPromotion_AlreadyApplied_ReturnsFailure()
    {
        var order = new Order { Id = 1, TableId = 1, SubTotal = 100m, DiscountAmount = 10m, Items = new List<OrderItem>() };
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        var result = await _sut.ApplyPromotionAsync(1, new ApplyPromotionRequest("SAVE10"));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("A promotion is already applied to this order");
    }
}
