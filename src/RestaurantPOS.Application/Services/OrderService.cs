using Microsoft.Extensions.Logging;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Core.Common;
using RestaurantPOS.Core.DTOs;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Enums;
using RestaurantPOS.Core.Interfaces;

namespace RestaurantPOS.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IMenuItemRepository _menuRepo;
    private readonly ITableRepository _tableRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IPromotionRepository _promoRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepo,
        IMenuItemRepository menuRepo,
        ITableRepository tableRepo,
        IInventoryRepository inventoryRepo,
        IPromotionRepository promoRepo,
        IPaymentRepository paymentRepo,
        ILogger<OrderService> logger)
    {
        _orderRepo = orderRepo;
        _menuRepo = menuRepo;
        _tableRepo = tableRepo;
        _inventoryRepo = inventoryRepo;
        _promoRepo = promoRepo;
        _paymentRepo = paymentRepo;
        _logger = logger;
    }

    public async Task<Result<List<OrderResponse>>> GetAllAsync()
    {
        var orders = await _orderRepo.GetAllAsync();
        return Result<List<OrderResponse>>.Success(orders.Select(MapToResponse).ToList());
    }

    public async Task<Result<OrderResponse>> GetByIdAsync(int id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return Result<OrderResponse>.Failure("Order not found");
        return Result<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<Result<List<OrderResponse>>> GetByTableAsync(int tableId)
    {
        var orders = await _orderRepo.GetByTableAsync(tableId);
        return Result<List<OrderResponse>>.Success(orders.Select(MapToResponse).ToList());
    }

    public async Task<Result<List<OrderResponse>>> GetByStatusAsync(OrderStatus status)
    {
        var orders = await _orderRepo.GetByStatusAsync(status);
        return Result<List<OrderResponse>>.Success(orders.Select(MapToResponse).ToList());
    }

    public async Task<Result<OrderResponse>> CreateAsync(CreateOrderRequest request)
    {
        _logger.LogInformation("Creating Order for table {TableId}", request.TableId);
        var table = await _tableRepo.GetByIdAsync(request.TableId);
        if (table == null) return Result<OrderResponse>.Failure("Table not found");

        var order = new Order
        {
            TableId = request.TableId,
            SeatNumber = request.SeatNumber,
            WaiterId = request.WaiterId,
            WaiterName = request.WaiterName,
            CustomerId = request.CustomerId,
            Notes = request.Notes,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        decimal subTotal = 0;
        foreach (var itemReq in request.Items)
        {
            var menuItem = await _menuRepo.GetByIdAsync(itemReq.MenuItemId);
            if (menuItem == null)
                return Result<OrderResponse>.Failure($"Menu item {itemReq.MenuItemId} not found");
            if (!menuItem.IsAvailable)
                return Result<OrderResponse>.Failure($"{menuItem.Name} is not available");

            var orderItem = new OrderItem
            {
                MenuItemId = itemReq.MenuItemId,
                Quantity = itemReq.Quantity,
                UnitPrice = menuItem.Price,
                SpecialInstructions = itemReq.SpecialInstructions
            };
            order.Items.Add(orderItem);
            subTotal += orderItem.TotalPrice;

            await DeductInventory(menuItem, itemReq.Quantity);
        }

        order.SubTotal = subTotal;
        order.TaxAmount = Math.Round(subTotal * 0.08m, 2);

        var created = await _orderRepo.CreateAsync(order);

        table.Status = TableStatus.Occupied;
        await _tableRepo.UpdateAsync(table);

        return Result<OrderResponse>.Success(MapToResponse(created));
    }

    public async Task<Result<OrderResponse>> UpdateStatusAsync(int id, UpdateOrderRequest request)
    {
        _logger.LogInformation("Updating Order {Id} status to {Status}", id, request.Status);
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return Result<OrderResponse>.Failure("Order not found");

        if (request.Status.HasValue) order.Status = request.Status.Value;
        if (request.Notes != null) order.Notes = request.Notes;

        await _orderRepo.UpdateAsync(order);

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
        {
            var table = await _tableRepo.GetByIdAsync(order.TableId);
            if (table != null)
            {
                table.Status = TableStatus.Available;
                await _tableRepo.UpdateAsync(table);
            }
        }

        return Result<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<Result<List<OrderResponse>>> SplitBillAsync(SplitBillRequest request)
    {
        var originalOrder = await _orderRepo.GetByIdAsync(request.OrderId);
        if (originalOrder == null)
            return Result<List<OrderResponse>>.Failure("Order not found");

        var results = new List<OrderResponse>();

        foreach (var group in request.Groups)
        {
            var splitOrder = new Order
            {
                TableId = originalOrder.TableId,
                SeatNumber = originalOrder.SeatNumber,
                WaiterId = originalOrder.WaiterId,
                WaiterName = originalOrder.WaiterName,
                CustomerId = group.CustomerId,
                Status = originalOrder.Status,
                CreatedAt = DateTime.UtcNow
            };

            decimal subTotal = 0;
            foreach (var itemId in group.ItemIds)
            {
                var origItem = originalOrder.Items.FirstOrDefault(i => i.Id == itemId);
                if (origItem != null)
                {
                    var splitItem = new OrderItem
                    {
                        MenuItemId = origItem.MenuItemId,
                        Quantity = origItem.Quantity,
                        UnitPrice = origItem.UnitPrice,
                        SpecialInstructions = origItem.SpecialInstructions
                    };
                    splitOrder.Items.Add(splitItem);
                    subTotal += splitItem.TotalPrice;
                    origItem.IsVoided = true;
                }
            }

            splitOrder.SubTotal = subTotal;
            splitOrder.TaxAmount = Math.Round(subTotal * 0.08m, 2);

            var created = await _orderRepo.CreateAsync(splitOrder);
            results.Add(MapToResponse(created));
        }

        originalOrder.SubTotal = originalOrder.Items.Where(i => !i.IsVoided).Sum(i => i.TotalPrice);
        originalOrder.TaxAmount = Math.Round(originalOrder.SubTotal * 0.08m, 2);
        await _orderRepo.UpdateAsync(originalOrder);

        return Result<List<OrderResponse>>.Success(results);
    }

    public async Task<Result<bool>> VoidItemAsync(int orderId, int orderItemId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return Result<bool>.Failure("Order not found");

        var item = order.Items.FirstOrDefault(i => i.Id == orderItemId);
        if (item == null) return Result<bool>.Failure("Order item not found");

        item.IsVoided = true;
        order.SubTotal = order.Items.Where(i => !i.IsVoided).Sum(i => i.TotalPrice);
        order.TaxAmount = Math.Round(order.SubTotal * 0.08m, 2);
        await _orderRepo.UpdateAsync(order);

        return Result<bool>.Success(true);
    }

    public async Task<Result<OrderResponse>> ApplyPromotionAsync(int orderId, ApplyPromotionRequest request)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return Result<OrderResponse>.Failure("Order not found");

        if (order.DiscountAmount > 0)
            return Result<OrderResponse>.Failure("A promotion is already applied to this order");

        var promo = await _promoRepo.GetByCodeAsync(request.PromotionCode);
        if (promo == null) return Result<OrderResponse>.Failure("Promotion not found");
        if (!promo.IsActive) return Result<OrderResponse>.Failure("Promotion is expired or not yet active");
        if (promo.MaxUses.HasValue && promo.CurrentUses >= promo.MaxUses.Value)
            return Result<OrderResponse>.Failure("Promotion usage limit reached");

        decimal discount = promo.IsPercentBased
            ? Math.Round(order.SubTotal * (promo.DiscountPercent / 100m), 2)
            : promo.DiscountAmount;

        if (discount > order.SubTotal)
            discount = order.SubTotal;

        order.DiscountAmount = discount;
        promo.CurrentUses++;
        await _promoRepo.UpdateAsync(promo);
        await _orderRepo.UpdateAsync(order);

        return Result<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<Result<ReceiptResponse>> GetReceiptAsync(int orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return Result<ReceiptResponse>.Failure("Order not found");

        var payment = await _paymentRepo.GetByOrderIdAsync(orderId);

        var receiptItems = order.Items.Where(i => !i.IsVoided).Select(i => new ReceiptItemResponse(
            i.MenuItem?.Name ?? "",
            i.Quantity,
            i.UnitPrice,
            i.TotalPrice,
            i.SpecialInstructions
        )).ToList();

        var receipt = new ReceiptResponse(
            OrderId: order.Id,
            TableNumber: order.Table?.TableNumber ?? "",
            WaiterName: order.WaiterName,
            Items: receiptItems,
            SubTotal: order.SubTotal,
            TaxAmount: order.TaxAmount,
            DiscountAmount: order.DiscountAmount,
            TipAmount: order.TipAmount,
            TotalAmount: order.TotalAmount,
            PaymentMethod: payment?.Method.ToString() ?? "Unpaid",
            PaymentStatus: payment?.Status.ToString() ?? "Pending",
            CreatedAt: order.CreatedAt,
            PaidAt: payment?.PaidAt
        );

        return Result<ReceiptResponse>.Success(receipt);
    }

    private async Task DeductInventory(MenuItem menuItem, int quantity)
    {
        foreach (var link in menuItem.MenuItemInventories)
        {
            var inventoryItem = link.InventoryItem;
            if (inventoryItem == null) continue;
            var deduction = (int)(link.QuantityRequired * quantity);
            inventoryItem.Quantity = Math.Max(0, inventoryItem.Quantity - deduction);
            await _inventoryRepo.UpdateAsync(inventoryItem);
        }
    }

    public async Task<Result<PaginatedResponse<OrderResponse>>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _orderRepo.GetAllPaginatedAsync(request);
        return Result<PaginatedResponse<OrderResponse>>.Success(new PaginatedResponse<OrderResponse>
        {
            Items = result.Items.Select(MapToResponse).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        });
    }

    private static OrderResponse MapToResponse(Order order) =>
        new(order.Id, order.TableId, order.Table?.TableNumber ?? "",
            order.WaiterName, order.Status, order.SubTotal, order.TaxAmount,
            order.DiscountAmount, order.TipAmount, order.TotalAmount,
            order.SeatNumber, order.Notes, order.CreatedAt,
            order.Items.Select(i => new OrderItemResponse(
                i.Id, i.MenuItemId, i.MenuItem?.Name ?? "",
                i.Quantity, i.UnitPrice, i.TotalPrice,
                i.SpecialInstructions, i.IsVoided)).ToList());
}
