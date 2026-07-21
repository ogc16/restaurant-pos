using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Core.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalPaid => Amount + TipAmount;
    public DateTime? PaidAt { get; set; }
}
