using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context) => _context = context;

    public async Task<Payment?> GetByIdAsync(int id)
        => await _context.Payments.Include(p => p.Order).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Payment?> GetByOrderIdAsync(int orderId)
        => await _context.Payments.Include(p => p.Order).FirstOrDefaultAsync(p => p.OrderId == orderId);

    public async Task<List<Payment>> GetAllAsync()
        => await _context.Payments.Include(p => p.Order).AsNoTracking().OrderByDescending(p => p.CreatedAt).ToListAsync();

    public async Task<List<Payment>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _context.Payments
            .Include(p => p.Order)
            .Where(p => p.CreatedAt >= from && p.CreatedAt <= to)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Payment> CreateAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task UpdateAsync(Payment payment)
    {
        payment.UpdatedAt = DateTime.UtcNow;
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<PaginatedResponse<Payment>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var query = _context.Payments.Include(p => p.Order).AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedAt).Skip(request.Skip).Take(request.PageSize).ToListAsync();
        return new PaginatedResponse<Payment> { Items = items, TotalCount = totalCount, Page = request.Page, PageSize = request.PageSize };
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Set<Payment>().CountAsync();
    }
}
