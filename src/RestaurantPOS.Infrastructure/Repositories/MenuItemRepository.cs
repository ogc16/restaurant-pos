using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Repositories;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly AppDbContext _context;

    public MenuItemRepository(AppDbContext context) => _context = context;

    public async Task<List<MenuItem>> GetAllAsync()
        => await _context.MenuItems.AsNoTracking().ToListAsync();

    public async Task<List<MenuItem>> GetByCategoryAsync(string category)
    {
        if (Enum.TryParse<MenuItemCategory>(category, true, out var cat))
            return await _context.MenuItems.Where(m => m.Category == cat).AsNoTracking().ToListAsync();
        return await _context.MenuItems.AsNoTracking().ToListAsync();
    }

    public async Task<List<MenuItem>> GetDailySpecialsAsync()
        => await _context.MenuItems.Where(m => m.IsDailySpecial && m.IsAvailable).AsNoTracking().ToListAsync();

    public async Task<MenuItem?> GetByIdAsync(int id)
        => await _context.MenuItems
            .Include(m => m.MenuItemInventories).ThenInclude(mi => mi.InventoryItem)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<MenuItem> CreateAsync(MenuItem item)
    {
        _context.MenuItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateAsync(MenuItem item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _context.MenuItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _context.MenuItems.FindAsync(id);
        if (item != null)
        {
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PaginatedResponse<MenuItem>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var query = _context.Set<MenuItem>().AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(m => m.CreatedAt).Skip(request.Skip).Take(request.PageSize).ToListAsync();
        return new PaginatedResponse<MenuItem> { Items = items, TotalCount = totalCount, Page = request.Page, PageSize = request.PageSize };
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Set<MenuItem>().CountAsync();
    }
}
