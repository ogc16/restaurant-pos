using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Core.DTOs;

public record CreateMenuItemRequest(
    string Name,
    string Description,
    decimal Price,
    MenuItemCategory Category,
    string? ImageUrl,
    int PrepTimeMinutes,
    bool IsDailySpecial
);

public record UpdateMenuItemRequest(
    string? Name,
    string? Description,
    decimal? Price,
    MenuItemCategory? Category,
    string? ImageUrl,
    int? PrepTimeMinutes,
    bool? IsAvailable,
    bool? IsDailySpecial
);

public record MenuItemResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string? ImageUrl,
    MenuItemCategory Category,
    bool IsAvailable,
    bool IsDailySpecial,
    int PrepTimeMinutes
);
