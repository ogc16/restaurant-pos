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

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepoMock = new();
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<ICustomerRepository> _customerRepoMock = new();
    private readonly Mock<ILogger<PaymentService>> _loggerMock = new();
    private readonly PaymentService _sut;

    public PaymentServiceTests()
    {
        _sut = new PaymentService(
            _paymentRepoMock.Object,
            _orderRepoMock.Object,
            _customerRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ProcessPayment_ValidRequest_ReturnsPayment()
    {
        var order = new Order { Id = 1, TableId = 1, Status = OrderStatus.Served, SubTotal = 50m, Items = new List<OrderItem>() };
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _paymentRepoMock.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync((Payment?)null);

        var createdPayment = new Payment
        {
            Id = 1,
            OrderId = 1,
            Amount = 50m,
            Method = PaymentMethod.Cash,
            Status = PaymentStatus.Completed,
            TipAmount = 5m,
            PaidAt = DateTime.UtcNow
        };
        _paymentRepoMock.Setup(r => r.CreateAsync(It.IsAny<Payment>())).ReturnsAsync(createdPayment);

        var request = new ProcessPaymentRequest(1, PaymentMethod.Cash, 50m, 5m, null);

        var result = await _sut.ProcessPaymentAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Amount.Should().Be(50m);
        result.Data.Method.Should().Be(PaymentMethod.Cash);
        order.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public async Task ProcessPayment_OrderNotFound_ReturnsFailure()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Order?)null);

        var request = new ProcessPaymentRequest(99, PaymentMethod.Cash, 50m, 0m, null);

        var result = await _sut.ProcessPaymentAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Order not found");
    }

    [Fact]
    public async Task ProcessPayment_AlreadyPaid_ReturnsFailure()
    {
        var order = new Order { Id = 1, TableId = 1, Status = OrderStatus.Completed, Items = new List<OrderItem>() };
        var existingPayment = new Payment { Id = 1, OrderId = 1, Status = PaymentStatus.Completed };
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _paymentRepoMock.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync(existingPayment);

        var request = new ProcessPaymentRequest(1, PaymentMethod.Cash, 50m, 0m, null);

        var result = await _sut.ProcessPaymentAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Order already paid");
    }

    [Fact]
    public async Task ProcessPayment_WithCustomer_AwardsLoyaltyPoints()
    {
        var customer = new Customer { Id = 1, FullName = "John", LoyaltyPoints = 0 };
        var order = new Order { Id = 1, TableId = 1, CustomerId = 1, Status = OrderStatus.Served, SubTotal = 50m, Items = new List<OrderItem>() };
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _paymentRepoMock.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync((Payment?)null);
        _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        var createdPayment = new Payment { Id = 1, OrderId = 1, Amount = 50m, Method = PaymentMethod.Cash, Status = PaymentStatus.Completed, TipAmount = 0m, PaidAt = DateTime.UtcNow };
        _paymentRepoMock.Setup(r => r.CreateAsync(It.IsAny<Payment>())).ReturnsAsync(createdPayment);

        var request = new ProcessPaymentRequest(1, PaymentMethod.Cash, 50m, 0m, null);

        var result = await _sut.ProcessPaymentAsync(request);

        result.IsSuccess.Should().BeTrue();
        customer.LoyaltyPoints.Should().Be(5);
        _customerRepoMock.Verify(r => r.UpdateAsync(customer), Times.Once);
    }

    [Fact]
    public async Task GetByOrderId_ExistingPayment_ReturnsPayment()
    {
        var payment = new Payment
        {
            Id = 1,
            OrderId = 1,
            Amount = 50m,
            Method = PaymentMethod.CreditCard,
            Status = PaymentStatus.Completed,
            TipAmount = 5m,
            PaidAt = DateTime.UtcNow
        };
        _paymentRepoMock.Setup(r => r.GetByOrderIdAsync(1)).ReturnsAsync(payment);

        var result = await _sut.GetByOrderIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.OrderId.Should().Be(1);
        result.Data.Method.Should().Be(PaymentMethod.CreditCard);
    }
}
