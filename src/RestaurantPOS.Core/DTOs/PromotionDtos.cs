namespace RestaurantPOS.Core.DTOs;

public record CreatePromotionRequest(
    string Code,
    string Description,
    decimal DiscountPercent,
    decimal DiscountAmount,
    bool IsPercentBased,
    DateTime ValidFrom,
    DateTime ValidUntil,
    int? MaxUses
);

public record UpdatePromotionRequest(
    string? Code,
    string? Description,
    decimal? DiscountPercent,
    decimal? DiscountAmount,
    bool? IsPercentBased,
    DateTime? ValidFrom,
    DateTime? ValidUntil,
    int? MaxUses
);

public record PromotionResponse(
    int Id,
    string Code,
    string Description,
    decimal DiscountPercent,
    decimal DiscountAmount,
    bool IsPercentBased,
    DateTime ValidFrom,
    DateTime ValidUntil,
    bool IsActive,
    int? MaxUses,
    int CurrentUses
);
