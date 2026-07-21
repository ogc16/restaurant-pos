using RestaurantPOS.Core.Common;

namespace RestaurantPOS.Core.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int LoyaltyPoints { get; set; }
    public string? PreferredNotes { get; set; }
    public List<Order> Orders { get; set; } = new();
    public List<Promotion> AppliedPromotions { get; set; } = new();
}
