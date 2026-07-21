using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;

    public PromotionRepository(AppDbContext context) => _context = context;

    public async Task<List<Promotion>> GetAllAsync()
        => await _context.Promotions.AsNoTracking().ToListAsync();

    public async Task<Promotion?> GetByIdAsync(int id)
        => await _context.Promotions.FindAsync(id);

    public async Task<Promotion?> GetByCodeAsync(string code)
        => await _context.Promotions.FirstOrDefaultAsync(p => p.Code == code);

    public async Task<Promotion> CreateAsync(Promotion promotion)
    {
        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();
        return promotion;
    }

    public async Task UpdateAsync(Promotion promotion)
    {
        promotion.UpdatedAt = DateTime.UtcNow;
        _context.Promotions.Update(promotion);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var promo = await _context.Promotions.FindAsync(id);
        if (promo != null)
        {
            promo.IsDeleted = true;
            promo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PaginatedResponse<Promotion>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var query = _context.Set<Promotion>().AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedAt).Skip(request.Skip).Take(request.PageSize).ToListAsync();
        return new PaginatedResponse<Promotion> { Items = items, TotalCount = totalCount, Page = request.Page, PageSize = request.PageSize };
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Set<Promotion>().CountAsync();
    }
}
