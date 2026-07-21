using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Core.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetByTableAsync(int tableId);
    Task<List<Order>> GetByStatusAsync(OrderStatus status);
    Task<List<Order>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<Order> CreateAsync(Order order);
    Task UpdateAsync(Order order);
    Task<List<Order>> GetOrdersByWaiterAsync(string waiterId);
}
