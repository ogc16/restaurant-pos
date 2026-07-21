using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _service;

    public PromotionsController(IPromotionService service) => _service = service;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<PromotionResponse>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<PromotionResponse>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize]
    [HttpGet("validate/{code}")]
    public async Task<IActionResult> ValidateCode(string code)
    {
        var result = await _service.ValidateCodeAsync(code);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<PromotionResponse>> Create([FromBody] CreatePromotionRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : BadRequest(result.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePromotionRequest request)
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
