using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using RestaurantPOS.Application.Services;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Tests;

public class PromotionServiceTests
{
    private readonly Mock<IPromotionRepository> _repoMock = new();
    private readonly Mock<ILogger<PromotionService>> _loggerMock = new();
    private readonly PromotionService _sut;

    public PromotionServiceTests()
    {
        _sut = new PromotionService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsPromotions()
    {
        var promos = new List<Promotion>
        {
            new() { Id = 1, Code = "SAVE10", Description = "10% off", DiscountPercent = 10, IsPercentBased = true, ValidFrom = DateTime.UtcNow.AddDays(-1), ValidUntil = DateTime.UtcNow.AddDays(30) },
            new() { Id = 2, Code = "FLAT5", Description = "$5 off", DiscountAmount = 5, IsPercentBased = false, ValidFrom = DateTime.UtcNow.AddDays(-1), ValidUntil = DateTime.UtcNow.AddDays(30) }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(promos);

        var result = await _sut.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsPromotion()
    {
        var request = new CreatePromotionRequest("WELCOME", "Welcome discount", 15, 0, true, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null);
        var created = new Promotion { Id = 1, Code = "WELCOME", Description = "Welcome discount", DiscountPercent = 15, IsPercentBased = true, ValidFrom = DateTime.UtcNow, ValidUntil = DateTime.UtcNow.AddDays(30) };
        _repoMock.Setup(r => r.GetByCodeAsync("WELCOME")).ReturnsAsync((Promotion?)null);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Promotion>())).ReturnsAsync(created);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Code.Should().Be("WELCOME");
        result.Data.DiscountPercent.Should().Be(15);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Promotion>()), Times.Once);
    }

    [Fact]
    public async Task Create_DuplicateCode_ReturnsFailure()
    {
        var existing = new Promotion { Id = 1, Code = "SAVE10" };
        _repoMock.Setup(r => r.GetByCodeAsync("SAVE10")).ReturnsAsync(existing);

        var request = new CreatePromotionRequest("SAVE10", "Duplicate", 10, 0, true, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), null);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Promotion code already exists");
    }

    [Fact]
    public async Task ValidateCode_ValidActiveCode_ReturnsPromotion()
    {
        var promo = new Promotion
        {
            Id = 1,
            Code = "SAVE10",
            Description = "10% off",
            DiscountPercent = 10,
            IsPercentBased = true,
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidUntil = DateTime.UtcNow.AddDays(30),
            MaxUses = 100,
            CurrentUses = 5
        };
        _repoMock.Setup(r => r.GetByCodeAsync("SAVE10")).ReturnsAsync(promo);

        var result = await _sut.ValidateCodeAsync("SAVE10");

        result.IsSuccess.Should().BeTrue();
        result.Data!.Code.Should().Be("SAVE10");
    }

    [Fact]
    public async Task ValidateCode_ExpiredCode_ReturnsFailure()
    {
        var promo = new Promotion
        {
            Id = 1,
            Code = "EXPIRED",
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidUntil = DateTime.UtcNow.AddDays(-1)
        };
        _repoMock.Setup(r => r.GetByCodeAsync("EXPIRED")).ReturnsAsync(promo);

        var result = await _sut.ValidateCodeAsync("EXPIRED");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Promotion is expired or not yet active");
    }

    [Fact]
    public async Task ValidateCode_UsageLimitReached_ReturnsFailure()
    {
        var promo = new Promotion
        {
            Id = 1,
            Code = "LIMITED",
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidUntil = DateTime.UtcNow.AddDays(30),
            MaxUses = 10,
            CurrentUses = 10
        };
        _repoMock.Setup(r => r.GetByCodeAsync("LIMITED")).ReturnsAsync(promo);

        var result = await _sut.ValidateCodeAsync("LIMITED");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Promotion usage limit reached");
    }

    [Fact]
    public async Task Update_ExistingPromotion_ReturnsUpdatedPromotion()
    {
        var promo = new Promotion { Id = 1, Code = "SAVE10", Description = "10% off", DiscountPercent = 10, IsPercentBased = true, ValidFrom = DateTime.UtcNow, ValidUntil = DateTime.UtcNow.AddDays(30) };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(promo);

        var request = new UpdatePromotionRequest(null, "Updated description", null, null, null, null, null, null);

        var result = await _sut.UpdateAsync(1, request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Description.Should().Be("Updated description");
        _repoMock.Verify(r => r.UpdateAsync(promo), Times.Once);
    }

    [Fact]
    public async Task Delete_ExistingPromotion_ReturnsSuccess()
    {
        var promo = new Promotion { Id = 1, Code = "SAVE10" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(promo);

        var result = await _sut.DeleteAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
