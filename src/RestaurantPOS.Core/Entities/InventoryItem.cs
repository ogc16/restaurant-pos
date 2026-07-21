using RestaurantPOS.Core.Common;

namespace RestaurantPOS.Core.Entities;

public class InventoryItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int LowStockThreshold { get; set; } = 10;
    public bool IsOutOfStock => Quantity <= 0;
    public bool IsLowStock => Quantity <= LowStockThreshold && Quantity > 0;
    public string? SupplierName { get; set; }
    public string? SupplierContact { get; set; }
    public decimal CostPerUnit { get; set; }
    public List<MenuItemInventory> MenuItemInventories { get; set; } = new();
    public List<RestockLog> RestockLogs { get; set; } = new();
}
