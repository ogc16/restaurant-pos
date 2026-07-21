using FluentValidation;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Validation;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.TableId).GreaterThan(0).WithMessage("Table ID is required");
        RuleFor(x => x.SeatNumber).GreaterThan(0).WithMessage("Seat number is required");
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one item is required");
        RuleFor(x => x.Items).Must(items => items.All(i => i.Quantity > 0))
            .WithMessage("All items must have quantity greater than 0");
    }
}
