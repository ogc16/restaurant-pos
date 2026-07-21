using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Core.DTOs;

public record CreateOrderRequest(
    int TableId,
    int SeatNumber,
    string? WaiterId,
    string? WaiterName,
    int? CustomerId,
    List<CreateOrderItemRequest> Items,
    string? Notes
);

public record CreateOrderItemRequest(
    int MenuItemId,
    int Quantity,
    string? SpecialInstructions
);

public record UpdateOrderRequest(
    OrderStatus? Status,
    string? Notes
);

public record OrderResponse(
    int Id,
    int TableId,
    string TableNumber,
    string? WaiterName,
    OrderStatus Status,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TipAmount,
    decimal TotalAmount,
    int SeatNumber,
    string? Notes,
    DateTime CreatedAt,
    List<OrderItemResponse> Items
);

public record OrderItemResponse(
    int Id,
    int MenuItemId,
    string MenuItemName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string? SpecialInstructions,
    bool IsVoided
);

public record SplitBillRequest(
    int OrderId,
    List<SplitBillGroup> Groups
);

public record SplitBillGroup(
    List<int> ItemIds,
    int? CustomerId
);

public record DailySalesReport(
    DateTime Date,
    int TotalOrders,
    decimal TotalRevenue,
    decimal TotalTips,
    int ItemsSold,
    List<TopSellingItem> TopSellingItems
);

public record ApplyPromotionRequest(
    string PromotionCode
);

public record TopSellingItem(
    int MenuItemId,
    string Name,
    int QuantitySold,
    decimal Revenue
);

public record ReceiptItemResponse(
    string MenuItemName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string? SpecialInstructions
);

public record ReceiptResponse(
    int OrderId,
    string TableNumber,
    string? WaiterName,
    List<ReceiptItemResponse> Items,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TipAmount,
    decimal TotalAmount,
    string PaymentMethod,
    string PaymentStatus,
    DateTime CreatedAt,
    DateTime? PaidAt
);
