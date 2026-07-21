using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Core.DTOs;

public record CreateTableRequest(
    string TableNumber,
    int Capacity,
    int Seats,
    string? Section
);

public record UpdateTableRequest(
    string? TableNumber,
    int? Capacity,
    int? Seats,
    string? Section,
    TableStatus? Status
);

public record TableResponse(
    int Id,
    string TableNumber,
    int Capacity,
    TableStatus Status,
    string? Section,
    int Seats,
    int? CurrentOrderId
);
