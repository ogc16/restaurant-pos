namespace RestaurantPOS.Core.DTOs;

public record CreateCustomerRequest(
    string FullName,
    string? Email,
    string? Phone,
    string? PreferredNotes
);

public record UpdateCustomerRequest(
    string? FullName,
    string? Email,
    string? Phone,
    string? PreferredNotes
);

public record CustomerResponse(
    int Id,
    string FullName,
    string? Email,
    string? Phone,
    int LoyaltyPoints,
    string? PreferredNotes
);
