using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Interfaces;

public interface IInventoryService
{
    Task<Result<List<InventoryItemResponse>>> GetAllAsync();
    Task<Result<InventoryItemResponse>> GetByIdAsync(int id);
    Task<Result<List<InventoryItemResponse>>> GetLowStockAsync();
    Task<Result<List<InventoryItemResponse>>> GetOutOfStockAsync();
    Task<Result<InventoryItemResponse>> CreateAsync(CreateInventoryItemRequest request);
    Task<Result<InventoryItemResponse>> UpdateAsync(int id, UpdateInventoryItemRequest request);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<bool>> RestockAsync(RestockRequest request);
}
