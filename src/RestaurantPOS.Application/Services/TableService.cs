using Microsoft.Extensions.Logging;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _repository;
    private readonly ILogger<TableService> _logger;

    public TableService(ITableRepository repository, ILogger<TableService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<List<TableResponse>>> GetAllAsync()
    {
        var tables = await _repository.GetAllAsync();
        return Result<List<TableResponse>>.Success(tables.Select(MapToResponse).ToList());
    }

    public async Task<Result<List<TableResponse>>> GetAvailableAsync()
    {
        var tables = await _repository.GetAvailableAsync();
        return Result<List<TableResponse>>.Success(tables.Select(MapToResponse).ToList());
    }

    public async Task<Result<TableResponse>> GetByIdAsync(int id)
    {
        var table = await _repository.GetByIdAsync(id);
        if (table == null) return Result<TableResponse>.Failure("Table not found");
        return Result<TableResponse>.Success(MapToResponse(table));
    }

    public async Task<Result<TableResponse>> CreateAsync(CreateTableRequest request)
    {
        _logger.LogInformation("Creating Table: {TableNumber}", request.TableNumber);
        var table = new Table
        {
            TableNumber = request.TableNumber,
            Capacity = request.Capacity,
            Seats = request.Seats,
            Section = request.Section
        };
        var created = await _repository.CreateAsync(table);
        return Result<TableResponse>.Success(MapToResponse(created));
    }

    public async Task<Result<TableResponse>> UpdateAsync(int id, UpdateTableRequest request)
    {
        _logger.LogInformation("Updating Table {Id}", id);
        var table = await _repository.GetByIdAsync(id);
        if (table == null) return Result<TableResponse>.Failure("Table not found");

        if (request.TableNumber != null) table.TableNumber = request.TableNumber;
        if (request.Capacity.HasValue) table.Capacity = request.Capacity.Value;
        if (request.Seats.HasValue) table.Seats = request.Seats.Value;
        if (request.Section != null) table.Section = request.Section;
        if (request.Status.HasValue) table.Status = request.Status.Value;

        await _repository.UpdateAsync(table);
        return Result<TableResponse>.Success(MapToResponse(table));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting Table {Id}", id);
        var table = await _repository.GetByIdAsync(id);
        if (table == null) return Result<bool>.Failure("Table not found");
        await _repository.DeleteAsync(id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PaginatedResponse<TableResponse>>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _repository.GetAllPaginatedAsync(request);
        return Result<PaginatedResponse<TableResponse>>.Success(new PaginatedResponse<TableResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        });
    }

    private static TableResponse MapToResponse(Table t) =>
        new(t.Id, t.TableNumber, t.Capacity, t.Status, t.Section, t.Seats,
            t.Orders.FirstOrDefault(o =>
                o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled)?.Id);
}
