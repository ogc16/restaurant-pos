using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuItemsController : ControllerBase
{
    private readonly IMenuItemService _service;

    public MenuItemsController(IMenuItemService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<MenuItemResponse>>> GetAll() =>
        Ok((await _service.GetAllAsync()).Data);

    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<MenuItemResponse>>> GetByCategory(string category) =>
        Ok((await _service.GetByCategoryAsync(category)).Data);

    [HttpGet("specials")]
    public async Task<ActionResult<List<MenuItemResponse>>> GetDailySpecials() =>
        Ok((await _service.GetDailySpecialsAsync()).Data);

    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItemResponse>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<MenuItemResponse>> Create([FromBody] CreateMenuItemRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<MenuItemResponse>> Update(int id, [FromBody] UpdateMenuItemRequest request)
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
    [HttpPatch("{id}/availability")]
    public async Task<IActionResult> ToggleAvailability(int id)
    {
        var result = await _service.ToggleAvailabilityAsync(id);
        return result.IsSuccess ? Ok() : NotFound(result.Error);
    }
}
