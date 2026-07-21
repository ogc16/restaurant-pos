using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Application.Interfaces;

public interface IOrderService
{
    Task<Result<List<OrderResponse>>> GetAllAsync();
    Task<Result<OrderResponse>> GetByIdAsync(int id);
    Task<Result<List<OrderResponse>>> GetByTableAsync(int tableId);
    Task<Result<List<OrderResponse>>> GetByStatusAsync(OrderStatus status);
    Task<Result<OrderResponse>> CreateAsync(CreateOrderRequest request);
    Task<Result<OrderResponse>> UpdateStatusAsync(int id, UpdateOrderRequest request);
    Task<Result<List<OrderResponse>>> SplitBillAsync(SplitBillRequest request);
    Task<Result<bool>> VoidItemAsync(int orderId, int orderItemId);
    Task<Result<OrderResponse>> ApplyPromotionAsync(int orderId, ApplyPromotionRequest request);
    Task<Result<ReceiptResponse>> GetReceiptAsync(int orderId);
    Task<Result<PaginatedResponse<OrderResponse>>> GetAllPaginatedAsync(PaginatedRequest request);
}
