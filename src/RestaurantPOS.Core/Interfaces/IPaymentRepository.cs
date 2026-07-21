using RestaurantPOS.Core.Entities;

namespace RestaurantPOS.Core.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<Payment?> GetByOrderIdAsync(int orderId);
    Task<List<Payment>> GetAllAsync();
    Task<List<Payment>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<Payment> CreateAsync(Payment payment);
    Task UpdateAsync(Payment payment);
}
