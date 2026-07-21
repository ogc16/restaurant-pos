using Microsoft.Extensions.Logging;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<List<CustomerResponse>>> GetAllAsync()
    {
        var customers = await _repository.GetAllAsync();
        return Result<List<CustomerResponse>>.Success(customers.Select(MapToResponse).ToList());
    }

    public async Task<Result<CustomerResponse>> GetByIdAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null) return Result<CustomerResponse>.Failure("Customer not found");
        return Result<CustomerResponse>.Success(MapToResponse(customer));
    }

    public async Task<Result<CustomerResponse>> CreateAsync(CreateCustomerRequest request)
    {
        _logger.LogInformation("Creating Customer: {Name}", request.FullName);
        if (!string.IsNullOrEmpty(request.Email))
        {
            var existing = await _repository.GetByEmailAsync(request.Email);
            if (existing != null) return Result<CustomerResponse>.Failure("Email already registered");
        }

        if (!string.IsNullOrEmpty(request.Phone))
        {
            var existing = await _repository.GetByPhoneAsync(request.Phone);
            if (existing != null) return Result<CustomerResponse>.Failure("Phone number already registered");
        }

        var customer = new Customer
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PreferredNotes = request.PreferredNotes
        };
        var created = await _repository.CreateAsync(customer);
        return Result<CustomerResponse>.Success(MapToResponse(created));
    }

    public async Task<Result<CustomerResponse>> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        _logger.LogInformation("Updating Customer {Id}", id);
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null) return Result<CustomerResponse>.Failure("Customer not found");

        if (request.FullName != null) customer.FullName = request.FullName;
        if (request.Email != null) customer.Email = request.Email;
        if (request.Phone != null) customer.Phone = request.Phone;
        if (request.PreferredNotes != null) customer.PreferredNotes = request.PreferredNotes;

        await _repository.UpdateAsync(customer);
        return Result<CustomerResponse>.Success(MapToResponse(customer));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting Customer {Id}", id);
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null) return Result<bool>.Failure("Customer not found");
        await _repository.DeleteAsync(id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<CustomerResponse>> RedeemLoyaltyPointsAsync(int customerId, int points)
    {
        var customer = await _repository.GetByIdAsync(customerId);
        if (customer == null) return Result<CustomerResponse>.Failure("Customer not found");
        if (customer.LoyaltyPoints < points)
            return Result<CustomerResponse>.Failure("Insufficient loyalty points");

        customer.LoyaltyPoints -= points;
        await _repository.UpdateAsync(customer);
        return Result<CustomerResponse>.Success(MapToResponse(customer));
    }

    public async Task<Result<PaginatedResponse<CustomerResponse>>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _repository.GetAllPaginatedAsync(request);
        return Result<PaginatedResponse<CustomerResponse>>.Success(new PaginatedResponse<CustomerResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        });
    }

    private static CustomerResponse MapToResponse(Customer c) =>
        new(c.Id, c.FullName, c.Email, c.Phone, c.LoyaltyPoints, c.PreferredNotes);
}
