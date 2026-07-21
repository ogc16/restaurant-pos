using RestaurantPOS.Core.Entities;

namespace RestaurantPOS.Core.Interfaces;

public interface IInventoryRepository
{
    Task<List<InventoryItem>> GetAllAsync();
    Task<InventoryItem?> GetByIdAsync(int id);
    Task<List<InventoryItem>> GetLowStockAsync();
    Task<List<InventoryItem>> GetOutOfStockAsync();
    Task<InventoryItem> CreateAsync(InventoryItem item);
    Task UpdateAsync(InventoryItem item);
    Task DeleteAsync(int id);
    Task<RestockLog> LogRestockAsync(RestockLog log);
}
