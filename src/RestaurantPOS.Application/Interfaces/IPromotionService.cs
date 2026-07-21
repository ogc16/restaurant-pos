using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Interfaces;

public interface IPromotionService
{
    Task<Result<List<PromotionResponse>>> GetAllAsync();
    Task<Result<PromotionResponse>> GetByIdAsync(int id);
    Task<Result<PromotionResponse>> CreateAsync(CreatePromotionRequest request);
    Task<Result<PromotionResponse>> UpdateAsync(int id, UpdatePromotionRequest request);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<PromotionResponse>> ValidateCodeAsync(string code);
    Task<Result<PaginatedResponse<PromotionResponse>>> GetAllPaginatedAsync(PaginatedRequest request);
}
