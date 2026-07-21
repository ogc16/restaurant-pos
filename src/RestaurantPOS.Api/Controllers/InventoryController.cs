using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;

    public InventoryController(IInventoryService service) => _service = service;

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<InventoryItemResponse>>> GetAll() =>
        Ok((await _service.GetAllAsync()).Data);

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItemResponse>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize]
    [HttpGet("low-stock")]
    public async Task<ActionResult<List<InventoryItemResponse>>> GetLowStock() =>
        Ok((await _service.GetLowStockAsync()).Data);

    [Authorize(Roles = "Admin")]
    [HttpGet("out-of-stock")]
    public async Task<ActionResult<List<InventoryItemResponse>>> GetOutOfStock() =>
        Ok((await _service.GetOutOfStockAsync()).Data);

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<InventoryItemResponse>> Create([FromBody] CreateInventoryItemRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<InventoryItemResponse>> Update(int id, [FromBody] UpdateInventoryItemRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("restock")]
    public async Task<IActionResult> Restock([FromBody] RestockRequest request)
    {
        var result = await _service.RestockAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
