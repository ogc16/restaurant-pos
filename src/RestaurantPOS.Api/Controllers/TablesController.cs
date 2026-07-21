using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly ITableService _service;

    public TablesController(ITableService service) => _service = service;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<TableResponse>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize]
    [HttpGet("available")]
    public async Task<ActionResult<List<TableResponse>>> GetAvailable()
    {
        var result = await _service.GetAvailableAsync();
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<TableResponse>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<TableResponse>> Create([FromBody] CreateTableRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : BadRequest(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTableRequest request)
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
}
