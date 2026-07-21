using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _context;

    public InventoryRepository(AppDbContext context) => _context = context;

    public async Task<List<InventoryItem>> GetAllAsync()
        => await _context.InventoryItems.AsNoTracking().ToListAsync();

    public async Task<InventoryItem?> GetByIdAsync(int id)
        => await _context.InventoryItems.FindAsync(id);

    public async Task<List<InventoryItem>> GetLowStockAsync()
        => await _context.InventoryItems.Where(i => i.Quantity <= i.LowStockThreshold && i.Quantity > 0).AsNoTracking().ToListAsync();

    public async Task<List<InventoryItem>> GetOutOfStockAsync()
        => await _context.InventoryItems.Where(i => i.Quantity <= 0).AsNoTracking().ToListAsync();

    public async Task<InventoryItem> CreateAsync(InventoryItem item)
    {
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateAsync(InventoryItem item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _context.InventoryItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item != null)
        {
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<RestockLog> LogRestockAsync(RestockLog log)
    {
        _context.RestockLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }
}
