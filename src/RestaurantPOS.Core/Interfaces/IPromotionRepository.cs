using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Entities;

namespace RestaurantPOS.Core.Interfaces;

public interface IPromotionRepository
{
    Task<List<Promotion>> GetAllAsync();
    Task<PaginatedResponse<Promotion>> GetAllPaginatedAsync(PaginatedRequest request);
    Task<Promotion?> GetByIdAsync(int id);
    Task<Promotion?> GetByCodeAsync(string code);
    Task<Promotion> CreateAsync(Promotion promotion);
    Task UpdateAsync(Promotion promotion);
    Task DeleteAsync(int id);
    Task<int> GetCountAsync();
}
