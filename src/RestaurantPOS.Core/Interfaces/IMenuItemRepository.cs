using RestaurantPOS.Core.Entities;

namespace RestaurantPOS.Core.Interfaces;

public interface IMenuItemRepository
{
    Task<List<MenuItem>> GetAllAsync();
    Task<List<MenuItem>> GetByCategoryAsync(string category);
    Task<List<MenuItem>> GetDailySpecialsAsync();
    Task<MenuItem?> GetByIdAsync(int id);
    Task<MenuItem> CreateAsync(MenuItem item);
    Task UpdateAsync(MenuItem item);
    Task DeleteAsync(int id);
}
