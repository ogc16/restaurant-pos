using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Repositories;

public class TableRepository : ITableRepository
{
    private readonly AppDbContext _context;

    public TableRepository(AppDbContext context) => _context = context;

    public async Task<List<Table>> GetAllAsync()
        => await _context.Tables.AsNoTracking().ToListAsync();

    public async Task<Table?> GetByIdAsync(int id)
        => await _context.Tables.FindAsync(id);

    public async Task<List<Table>> GetAvailableAsync()
        => await _context.Tables.Where(t => t.Status == TableStatus.Available).AsNoTracking().ToListAsync();

    public async Task<Table> CreateAsync(Table table)
    {
        _context.Tables.Add(table);
        await _context.SaveChangesAsync();
        return table;
    }

    public async Task UpdateAsync(Table table)
    {
        table.UpdatedAt = DateTime.UtcNow;
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table != null)
        {
            table.IsDeleted = true;
            table.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PaginatedResponse<Table>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var query = _context.Set<Table>().AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(t => t.Id).Skip(request.Skip).Take(request.PageSize).ToListAsync();
        return new PaginatedResponse<Table> { Items = items, TotalCount = totalCount, Page = request.Page, PageSize = request.PageSize };
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Set<Table>().CountAsync();
    }
}
