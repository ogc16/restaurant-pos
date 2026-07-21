using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Infrastructure.Data;

namespace RestaurantPOS.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context) => _context = context;

    public async Task<List<Customer>> GetAllAsync()
        => await _context.Customers.AsNoTracking().ToListAsync();

    public async Task<Customer?> GetByIdAsync(int id)
        => await _context.Customers.FindAsync(id);

    public async Task<Customer?> GetByEmailAsync(string email)
        => await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);

    public async Task<Customer?> GetByPhoneAsync(string phone)
        => await _context.Customers.FirstOrDefaultAsync(c => c.Phone == phone);

    public async Task<Customer> CreateAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            customer.IsDeleted = true;
            customer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PaginatedResponse<Customer>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var query = _context.Set<Customer>().AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(c => c.CreatedAt).Skip(request.Skip).Take(request.PageSize).ToListAsync();
        return new PaginatedResponse<Customer> { Items = items, TotalCount = totalCount, Page = request.Page, PageSize = request.PageSize };
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Set<Customer>().CountAsync();
    }
}
