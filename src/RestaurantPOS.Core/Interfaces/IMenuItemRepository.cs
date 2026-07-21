using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Entities;

namespace RestaurantPOS.Core.Interfaces;

public interface IMenuItemRepository
{
    Task<List<MenuItem>> GetAllAsync();
    Task<PaginatedResponse<MenuItem>> GetAllPaginatedAsync(PaginatedRequest request);
    Task<List<MenuItem>> GetByCategoryAsync(string category);
    Task<List<MenuItem>> GetDailySpecialsAsync();
    Task<MenuItem?> GetByIdAsync(int id);
    Task<MenuItem> CreateAsync(MenuItem item);
    Task UpdateAsync(MenuItem item);
    Task DeleteAsync(int id);
    Task<int> GetCountAsync();
}
