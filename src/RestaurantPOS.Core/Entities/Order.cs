using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Core.Entities;

public class Order : BaseEntity
{
    public int TableId { get; set; }
    public Table Table { get; set; } = null!;
    public string? WaiterId { get; set; }
    public string? WaiterName { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount => SubTotal + TaxAmount - DiscountAmount + TipAmount;
    public int SeatNumber { get; set; }
    public string? Notes { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public Payment? Payment { get; set; }
}
