using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repository;

    public CustomersController(ICustomerRepository repository) => _repository = repository;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAll()
    {
        var customers = await _repository.GetAllAsync();
        return Ok(customers.Select(c => new CustomerResponse(c.Id, c.FullName, c.Email,
            c.Phone, c.LoyaltyPoints, c.PreferredNotes)));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(new CustomerResponse(customer.Id, customer.FullName, customer.Email,
            customer.Phone, customer.LoyaltyPoints, customer.PreferredNotes));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PreferredNotes = request.PreferredNotes
        };
        var created = await _repository.CreateAsync(customer);
        return Ok(new CustomerResponse(created.Id, created.FullName, created.Email,
            created.Phone, created.LoyaltyPoints, created.PreferredNotes));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest request)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null) return NotFound();

        if (request.FullName != null) customer.FullName = request.FullName;
        if (request.Email != null) customer.Email = request.Email;
        if (request.Phone != null) customer.Phone = request.Phone;
        if (request.PreferredNotes != null) customer.PreferredNotes = request.PreferredNotes;

        await _repository.UpdateAsync(customer);
        return Ok();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
