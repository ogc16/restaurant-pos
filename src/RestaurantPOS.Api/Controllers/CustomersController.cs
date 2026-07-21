using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomersController(ICustomerService service) => _service = service;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : BadRequest(result.Error);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [Authorize]
    [HttpPost("{id}/redeem")]
    public async Task<IActionResult> RedeemPoints(int id, [FromQuery] int points)
    {
        var result = await _service.RedeemLoyaltyPointsAsync(id, points);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }
}
