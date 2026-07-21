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

public class TableServiceTests
{
    private readonly Mock<ITableRepository> _repoMock = new();
    private readonly Mock<ILogger<TableService>> _loggerMock = new();
    private readonly TableService _sut;

    public TableServiceTests()
    {
        _sut = new TableService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsTables()
    {
        var tables = new List<Table>
        {
            new() { Id = 1, TableNumber = "T1", Capacity = 4, Status = TableStatus.Available, Seats = 4 },
            new() { Id = 2, TableNumber = "T2", Capacity = 2, Status = TableStatus.Occupied, Seats = 2 }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tables);

        var result = await _sut.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_ExistingTable_ReturnsTable()
    {
        var table = new Table { Id = 1, TableNumber = "T1", Capacity = 4, Status = TableStatus.Available, Seats = 4, Orders = new List<Order>() };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(table);

        var result = await _sut.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TableNumber.Should().Be("T1");
        result.Data.Capacity.Should().Be(4);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsTable()
    {
        var request = new CreateTableRequest("T5", 6, 6, "Patio");
        var created = new Table { Id = 5, TableNumber = "T5", Capacity = 6, Seats = 6, Section = "Patio", Status = TableStatus.Available };
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Table>())).ReturnsAsync(created);

        var result = await _sut.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.TableNumber.Should().Be("T5");
        result.Data.Capacity.Should().Be(6);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Table>()), Times.Once);
    }

    [Fact]
    public async Task Update_ExistingTable_ReturnsUpdatedTable()
    {
        var table = new Table { Id = 1, TableNumber = "T1", Capacity = 4, Status = TableStatus.Available, Seats = 4, Orders = new List<Order>() };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(table);

        var request = new UpdateTableRequest(null, 6, null, "VIP", null);

        var result = await _sut.UpdateAsync(1, request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Capacity.Should().Be(6);
        result.Data.Section.Should().Be("VIP");
        _repoMock.Verify(r => r.UpdateAsync(table), Times.Once);
    }

    [Fact]
    public async Task Delete_ExistingTable_ReturnsSuccess()
    {
        var table = new Table { Id = 1, TableNumber = "T1" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(table);

        var result = await _sut.DeleteAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
