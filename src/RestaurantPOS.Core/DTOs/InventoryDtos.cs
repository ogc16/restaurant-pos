namespace RestaurantPOS.Core.DTOs;

public record CreateInventoryItemRequest(
    string Name,
    string? Description,
    int Quantity,
    string Unit,
    int LowStockThreshold,
    string? SupplierName,
    string? SupplierContact,
    decimal CostPerUnit
);

public record UpdateInventoryItemRequest(
    string? Name,
    string? Description,
    int? Quantity,
    string? Unit,
    int? LowStockThreshold,
    string? SupplierName,
    string? SupplierContact,
    decimal? CostPerUnit
);

public record RestockRequest(
    int InventoryItemId,
    int Quantity,
    decimal CostPerUnit,
    string? SupplierName,
    string? Notes,
    string RestockedBy
);

public record InventoryItemResponse(
    int Id,
    string Name,
    string? Description,
    int Quantity,
    string Unit,
    int LowStockThreshold,
    bool IsOutOfStock,
    bool IsLowStock,
    string? SupplierName,
    string? SupplierContact,
    decimal CostPerUnit
);
