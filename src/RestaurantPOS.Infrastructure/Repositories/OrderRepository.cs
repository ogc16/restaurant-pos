using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context) => _context = context;

    public async Task<List<Order>> GetAllAsync()
        => await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<Order?> GetByIdAsync(int id)
        => await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<List<Order>> GetByTableAsync(int tableId)
        => await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Where(o => o.TableId == tableId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Order>> GetByStatusAsync(OrderStatus status)
        => await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Where(o => o.Status == status)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Order>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(order.Id) ?? order;
    }

    public async Task UpdateAsync(Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Order>> GetOrdersByWaiterAsync(string waiterId)
        => await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Where(o => o.WaiterId == waiterId)
            .AsNoTracking()
            .ToListAsync();
}
