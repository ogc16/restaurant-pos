using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Interfaces;

public interface ICustomerService
{
    Task<Result<List<CustomerResponse>>> GetAllAsync();
    Task<Result<CustomerResponse>> GetByIdAsync(int id);
    Task<Result<CustomerResponse>> CreateAsync(CreateCustomerRequest request);
    Task<Result<CustomerResponse>> UpdateAsync(int id, UpdateCustomerRequest request);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<CustomerResponse>> RedeemLoyaltyPointsAsync(int customerId, int points);
    Task<Result<PaginatedResponse<CustomerResponse>>> GetAllPaginatedAsync(PaginatedRequest request);
}
