using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionRepository _repository;

    public PromotionsController(IPromotionRepository repository) => _repository = repository;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<PromotionResponse>>> GetAll()
    {
        var promos = await _repository.GetAllAsync();
        return Ok(promos.Select(p => new PromotionResponse(p.Id, p.Code, p.Description,
            p.DiscountPercent, p.DiscountAmount, p.IsPercentBased, p.ValidFrom, p.ValidUntil,
            p.IsActive, p.MaxUses, p.CurrentUses)));
    }

    [Authorize]
    [HttpGet("validate/{code}")]
    public async Task<IActionResult> ValidateCode(string code)
    {
        var promo = await _repository.GetByCodeAsync(code);
        if (promo == null || !promo.IsActive)
            return NotFound("Invalid or expired promotion code");

        if (promo.MaxUses.HasValue && promo.CurrentUses >= promo.MaxUses.Value)
            return BadRequest("Promotion usage limit reached");

        return Ok(new { promo.Code, promo.DiscountPercent, promo.DiscountAmount, promo.IsPercentBased });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePromotionRequest request)
    {
        var promotion = new Promotion
        {
            Code = request.Code,
            Description = request.Description,
            DiscountPercent = request.DiscountPercent,
            DiscountAmount = request.DiscountAmount,
            IsPercentBased = request.IsPercentBased,
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            MaxUses = request.MaxUses
        };
        var created = await _repository.CreateAsync(promotion);
        return Ok(new { created.Id, created.Code });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
