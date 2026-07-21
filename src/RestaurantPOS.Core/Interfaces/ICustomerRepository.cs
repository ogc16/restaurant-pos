using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Entities;

namespace RestaurantPOS.Core.Interfaces;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task<PaginatedResponse<Customer>> GetAllPaginatedAsync(PaginatedRequest request);
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByPhoneAsync(string phone);
    Task<Customer> CreateAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
    Task<int> GetCountAsync();
}
