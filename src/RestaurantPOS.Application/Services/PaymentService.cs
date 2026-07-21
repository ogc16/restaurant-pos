using Microsoft.Extensions.Logging;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IPaymentRepository paymentRepo, IOrderRepository orderRepo, ICustomerRepository customerRepo, ILogger<PaymentService> logger)
    {
        _paymentRepo = paymentRepo;
        _orderRepo = orderRepo;
        _customerRepo = customerRepo;
        _logger = logger;
    }

    public async Task<Result<PaymentResponse>> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        _logger.LogInformation("Processing payment for Order {OrderId}, amount {Amount}", request.OrderId, request.Amount);
        var order = await _orderRepo.GetByIdAsync(request.OrderId);
        if (order == null)
            return Result<PaymentResponse>.Failure("Order not found");

        var existing = await _paymentRepo.GetByOrderIdAsync(request.OrderId);
        if (existing != null && existing.Status == PaymentStatus.Completed)
            return Result<PaymentResponse>.Failure("Order already paid");

        var payment = new Payment
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Method = request.Method,
            TipAmount = request.TipAmount,
            TransactionId = request.TransactionId,
            Status = PaymentStatus.Completed,
            PaidAt = DateTime.UtcNow
        };

        var created = await _paymentRepo.CreateAsync(payment);

        order.Status = OrderStatus.Completed;
        order.TipAmount = request.TipAmount;
        await _orderRepo.UpdateAsync(order);

        if (order.CustomerId.HasValue)
        {
            var customer = await _customerRepo.GetByIdAsync(order.CustomerId.Value);
            if (customer != null)
            {
                customer.LoyaltyPoints += (int)(request.Amount / 10);
                await _customerRepo.UpdateAsync(customer);
            }
        }

        return Result<PaymentResponse>.Success(MapToResponse(created));
    }

    public async Task<Result<PaymentResponse>> GetByOrderIdAsync(int orderId)
    {
        var payment = await _paymentRepo.GetByOrderIdAsync(orderId);
        if (payment == null) return Result<PaymentResponse>.Failure("Payment not found");
        return Result<PaymentResponse>.Success(MapToResponse(payment));
    }

    public async Task<Result<List<PaymentResponse>>> GetAllAsync()
    {
        var payments = await _paymentRepo.GetAllAsync();
        return Result<List<PaymentResponse>>.Success(payments.Select(MapToResponse).ToList());
    }

    public async Task<Result<PaginatedResponse<PaymentResponse>>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _paymentRepo.GetAllPaginatedAsync(request);
        return Result<PaginatedResponse<PaymentResponse>>.Success(new PaginatedResponse<PaymentResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        });
    }

    private static PaymentResponse MapToResponse(Payment p) =>
        new(p.Id, p.OrderId, p.Amount, p.Method, p.Status, p.TransactionId,
            p.TipAmount, p.TotalPaid, p.PaidAt);
}
