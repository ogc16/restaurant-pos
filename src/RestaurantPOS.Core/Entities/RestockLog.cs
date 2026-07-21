using RestaurantPOS.Core.Common;

namespace RestaurantPOS.Core.Entities;

public class RestockLog : BaseEntity
{
    public int InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;
    public int QuantityAdded { get; set; }
    public decimal CostPerUnit { get; set; }
    public decimal TotalCost => QuantityAdded * CostPerUnit;
    public string? SupplierName { get; set; }
    public string? Notes { get; set; }
    public string RestockedBy { get; set; } = string.Empty;
}
