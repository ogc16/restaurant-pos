using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using RestaurantPOS.Application.Services;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Tests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repoMock = new();
    private readonly Mock<ILogger<CustomerService>> _loggerMock = new();
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _sut = new CustomerService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsCustomers()
    {
        var customers = new List<Customer>
        {
            new() { Id = 1, FullName = "Alice", Email = "alice@test.com", LoyaltyPoints = 100 },
            new() { Id = 2, FullName = "Bob", Email = "bob@test.com", LoyaltyPoints = 50 }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

        var result = await _sut.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data![0].FullName.Should().Be("Alice");
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCustomer()
    {
        var request = new CreateCustomerRequest("Charlie", "charlie@test.com", "555-1234", "Prefers window");
        var created = new Customer { Id = 1, FullName = "Charlie", Email = "charlie@test.com", Phone = "555-1234", PreferredNotes = "Prefers window" };
        _repoMock.Setup(r => r.GetByEmailAsync("charlie@test.com")).ReturnsAsync((Customer?)null);
        _repoMock.Setup(r => r.GetByPhoneAsync("555-1234")).ReturnsAsync((Customer?)null);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Customer>())).ReturnsAsync(created);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.FullName.Should().Be("Charlie");
        result.Data.Email.Should().Be("charlie@test.com");
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task Create_DuplicateEmail_ReturnsFailure()
    {
        var existing = new Customer { Id = 1, FullName = "Alice", Email = "alice@test.com" };
        _repoMock.Setup(r => r.GetByEmailAsync("alice@test.com")).ReturnsAsync(existing);

        var request = new CreateCustomerRequest("Alice2", "alice@test.com", null, null);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Email already registered");
    }

    [Fact]
    public async Task Delete_ExistingCustomer_ReturnsSuccess()
    {
        var customer = new Customer { Id = 1, FullName = "Alice" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        var result = await _sut.DeleteAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task RedeemLoyaltyPoints_ValidPoints_ReducesBalance()
    {
        var customer = new Customer { Id = 1, FullName = "Alice", LoyaltyPoints = 100 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        var result = await _sut.RedeemLoyaltyPointsAsync(1, 30);

        result.IsSuccess.Should().BeTrue();
        customer.LoyaltyPoints.Should().Be(70);
        _repoMock.Verify(r => r.UpdateAsync(customer), Times.Once);
    }

    [Fact]
    public async Task RedeemLoyaltyPoints_InsufficientPoints_ReturnsFailure()
    {
        var customer = new Customer { Id = 1, FullName = "Alice", LoyaltyPoints = 10 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        var result = await _sut.RedeemLoyaltyPointsAsync(1, 50);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Insufficient loyalty points");
    }
}
