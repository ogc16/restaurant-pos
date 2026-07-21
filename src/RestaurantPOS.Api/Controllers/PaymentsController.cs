using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service) => _service = service;

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        var result = await _service.ProcessPaymentAsync(request);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }

    [Authorize]
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<PaymentResponse>> GetByOrderId(int orderId)
    {
        var result = await _service.GetByOrderIdAsync(orderId);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
    }

    [Authorize(Roles = "Admin,Cashier")]
    [HttpGet]
    public async Task<ActionResult<List<PaymentResponse>>> GetAll() =>
        Ok((await _service.GetAllAsync()).Data);
}
