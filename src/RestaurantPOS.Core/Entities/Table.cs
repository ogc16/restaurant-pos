using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Core.Entities;

public class Table : BaseEntity
{
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public TableStatus Status { get; set; } = TableStatus.Available;
    public string? Section { get; set; }
    public int Seats { get; set; }
    public List<Order> Orders { get; set; } = new();
}
