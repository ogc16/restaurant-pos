using RestaurantPOS.Core.Entities;

namespace RestaurantPOS.Core.Interfaces;

public interface ITableRepository
{
    Task<List<Table>> GetAllAsync();
    Task<Table?> GetByIdAsync(int id);
    Task<List<Table>> GetAvailableAsync();
    Task<Table> CreateAsync(Table table);
    Task UpdateAsync(Table table);
    Task DeleteAsync(int id);
}
