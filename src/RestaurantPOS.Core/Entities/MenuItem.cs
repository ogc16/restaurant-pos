using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Core.Entities;

public class MenuItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public MenuItemCategory Category { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsDailySpecial { get; set; }
    public int PrepTimeMinutes { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
    public List<MenuItemInventory> MenuItemInventories { get; set; } = new();
}
