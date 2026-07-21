using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Core.DTOs;

public record ProcessPaymentRequest(
    int OrderId,
    PaymentMethod Method,
    decimal Amount,
    decimal TipAmount,
    string? TransactionId
);

public record PaymentResponse(
    int Id,
    int OrderId,
    decimal Amount,
    PaymentMethod Method,
    PaymentStatus Status,
    string? TransactionId,
    decimal TipAmount,
    decimal TotalPaid,
    DateTime? PaidAt
);
