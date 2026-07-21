using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly ITableRepository _repository;

    public TablesController(ITableRepository repository) => _repository = repository;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<TableResponse>>> GetAll()
    {
        var tables = await _repository.GetAllAsync();
        return Ok(tables.Select(t => new TableResponse(t.Id, t.TableNumber, t.Capacity,
            t.Status, t.Section, t.Seats, t.Orders.FirstOrDefault(o =>
                o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled)?.Id)));
    }

    [Authorize]
    [HttpGet("available")]
    public async Task<ActionResult<List<TableResponse>>> GetAvailable()
    {
        var tables = await _repository.GetAvailableAsync();
        return Ok(tables.Select(t => new TableResponse(t.Id, t.TableNumber, t.Capacity,
            t.Status, t.Section, t.Seats, null)));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<TableResponse>> Create([FromBody] CreateTableRequest request)
    {
        var table = new Core.Entities.Table
        {
            TableNumber = request.TableNumber,
            Capacity = request.Capacity,
            Seats = request.Seats,
            Section = request.Section
        };
        var created = await _repository.CreateAsync(table);
        return CreatedAtAction(nameof(GetAll), new TableResponse(created.Id, created.TableNumber,
            created.Capacity, created.Status, created.Section, created.Seats, null));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTableRequest request)
    {
        var table = await _repository.GetByIdAsync(id);
        if (table == null) return NotFound();

        if (request.TableNumber != null) table.TableNumber = request.TableNumber;
        if (request.Capacity.HasValue) table.Capacity = request.Capacity.Value;
        if (request.Seats.HasValue) table.Seats = request.Seats.Value;
        if (request.Section != null) table.Section = request.Section;
        if (request.Status.HasValue) table.Status = request.Status.Value;

        await _repository.UpdateAsync(table);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
