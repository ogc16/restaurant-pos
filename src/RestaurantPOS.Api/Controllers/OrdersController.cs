using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Enums;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service) => _service = service;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAll() =>
        Ok((await _service.GetAllAsync()).Data);

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize]
    [HttpGet("table/{tableId}")]
    public async Task<ActionResult<List<OrderResponse>>> GetByTable(int tableId) =>
        Ok((await _service.GetByTableAsync(tableId)).Data);

    [Authorize]
    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<OrderResponse>>> GetByStatus(OrderStatus status) =>
        Ok((await _service.GetByStatusAsync(status)).Data);

    [Authorize(Roles = "Waiter,Admin")]
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : BadRequest(result.Error);
    }

    [Authorize]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(int id, [FromBody] UpdateOrderRequest request)
    {
        var result = await _service.UpdateStatusAsync(id, request);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize]
    [HttpPost("split")]
    public async Task<ActionResult<List<OrderResponse>>> SplitBill([FromBody] SplitBillRequest request)
    {
        var result = await _service.SplitBillAsync(request);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize]
    [HttpPatch("{orderId}/items/{orderItemId}/void")]
    public async Task<IActionResult> VoidItem(int orderId, int orderItemId)
    {
        var result = await _service.VoidItemAsync(orderId, orderItemId);
        return result.IsSuccess ? Ok() : NotFound(result.Error);
    }

    [Authorize]
    [HttpPost("{id}/apply-promotion")]
    public async Task<ActionResult<OrderResponse>> ApplyPromotion(int id, [FromBody] ApplyPromotionRequest request)
    {
        var result = await _service.ApplyPromotionAsync(id, request);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize]
    [HttpGet("{id}/receipt")]
    public async Task<IActionResult> GetReceipt(int id)
    {
        var result = await _service.GetReceiptAsync(id);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }
}
