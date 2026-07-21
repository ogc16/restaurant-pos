using RestaurantPOS.Core.Common;

namespace RestaurantPOS.Core.Entities;

public class Promotion : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public bool IsPercentBased { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool IsActive => DateTime.UtcNow >= ValidFrom && DateTime.UtcNow <= ValidUntil;
    public int? MaxUses { get; set; }
    public int CurrentUses { get; set; }
    public List<Customer> AppliedPromotions { get; set; } = new();
}
