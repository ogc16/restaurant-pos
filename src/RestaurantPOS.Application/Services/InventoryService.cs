using Microsoft.Extensions.Logging;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IInventoryRepository repository, ILogger<InventoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<List<InventoryItemResponse>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return Result<List<InventoryItemResponse>>.Success(items.Select(MapToResponse).ToList());
    }

    public async Task<Result<InventoryItemResponse>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return Result<InventoryItemResponse>.Failure("Inventory item not found");
        return Result<InventoryItemResponse>.Success(MapToResponse(item));
    }

    public async Task<Result<List<InventoryItemResponse>>> GetLowStockAsync()
    {
        var items = await _repository.GetLowStockAsync();
        return Result<List<InventoryItemResponse>>.Success(items.Select(MapToResponse).ToList());
    }

    public async Task<Result<List<InventoryItemResponse>>> GetOutOfStockAsync()
    {
        var items = await _repository.GetOutOfStockAsync();
        return Result<List<InventoryItemResponse>>.Success(items.Select(MapToResponse).ToList());
    }

    public async Task<Result<InventoryItemResponse>> CreateAsync(CreateInventoryItemRequest request)
    {
        _logger.LogInformation("Creating InventoryItem: {Name}", request.Name);
        var item = new InventoryItem
        {
            Name = request.Name,
            Description = request.Description,
            Quantity = request.Quantity,
            Unit = request.Unit,
            LowStockThreshold = request.LowStockThreshold,
            SupplierName = request.SupplierName,
            SupplierContact = request.SupplierContact,
            CostPerUnit = request.CostPerUnit
        };
        var created = await _repository.CreateAsync(item);
        return Result<InventoryItemResponse>.Success(MapToResponse(created));
    }

    public async Task<Result<InventoryItemResponse>> UpdateAsync(int id, UpdateInventoryItemRequest request)
    {
        _logger.LogInformation("Updating InventoryItem {Id}", id);
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return Result<InventoryItemResponse>.Failure("Inventory item not found");

        if (request.Name != null) item.Name = request.Name;
        if (request.Description != null) item.Description = request.Description;
        if (request.Quantity.HasValue) item.Quantity = request.Quantity.Value;
        if (request.Unit != null) item.Unit = request.Unit;
        if (request.LowStockThreshold.HasValue) item.LowStockThreshold = request.LowStockThreshold.Value;
        if (request.SupplierName != null) item.SupplierName = request.SupplierName;
        if (request.SupplierContact != null) item.SupplierContact = request.SupplierContact;
        if (request.CostPerUnit.HasValue) item.CostPerUnit = request.CostPerUnit.Value;

        await _repository.UpdateAsync(item);
        return Result<InventoryItemResponse>.Success(MapToResponse(item));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting InventoryItem {Id}", id);
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return Result<bool>.Failure("Inventory item not found");
        await _repository.DeleteAsync(id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RestockAsync(RestockRequest request)
    {
        var item = await _repository.GetByIdAsync(request.InventoryItemId);
        if (item == null) return Result<bool>.Failure("Inventory item not found");

        item.Quantity += request.Quantity;
        item.CostPerUnit = request.CostPerUnit;
        await _repository.UpdateAsync(item);

        var log = new RestockLog
        {
            InventoryItemId = request.InventoryItemId,
            QuantityAdded = request.Quantity,
            CostPerUnit = request.CostPerUnit,
            SupplierName = request.SupplierName,
            Notes = request.Notes,
            RestockedBy = request.RestockedBy
        };
        await _repository.LogRestockAsync(log);

        return Result<bool>.Success(true);
    }

    public async Task<Result<PaginatedResponse<InventoryItemResponse>>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _repository.GetAllPaginatedAsync(request);
        return Result<PaginatedResponse<InventoryItemResponse>>.Success(new PaginatedResponse<InventoryItemResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        });
    }

    private static InventoryItemResponse MapToResponse(InventoryItem i) =>
        new(i.Id, i.Name, i.Description, i.Quantity, i.Unit,
            i.LowStockThreshold, i.IsOutOfStock, i.IsLowStock,
            i.SupplierName, i.SupplierContact, i.CostPerUnit);
}
