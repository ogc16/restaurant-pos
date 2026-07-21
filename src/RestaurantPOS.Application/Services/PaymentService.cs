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

    public PaymentService(IPaymentRepository paymentRepo, IOrderRepository orderRepo, ICustomerRepository customerRepo)
    {
        _paymentRepo = paymentRepo;
        _orderRepo = orderRepo;
        _customerRepo = customerRepo;
    }

    public async Task<Result<PaymentResponse>> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
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

    private static PaymentResponse MapToResponse(Payment p) =>
        new(p.Id, p.OrderId, p.Amount, p.Method, p.Status, p.TransactionId,
            p.TipAmount, p.TotalPaid, p.PaidAt);
}
