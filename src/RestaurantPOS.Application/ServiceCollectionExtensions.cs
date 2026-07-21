using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RestaurantPOS.Application.Interfaces;
using RestaurantPOS.Application.Services;

namespace RestaurantPOS.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RestaurantPOS.Application.Validation.CreateOrderRequestValidator>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IReportingService, ReportingService>();
        return services;
    }
}
