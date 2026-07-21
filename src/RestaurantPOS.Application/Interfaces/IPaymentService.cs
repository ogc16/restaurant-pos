using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentResponse>> ProcessPaymentAsync(ProcessPaymentRequest request);
    Task<Result<PaymentResponse>> GetByOrderIdAsync(int orderId);
    Task<Result<List<PaymentResponse>>> GetAllAsync();
    Task<Result<PaginatedResponse<PaymentResponse>>> GetAllPaginatedAsync(PaginatedRequest request);
}
