using FluentValidation;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Validation;

public class CreateTableRequestValidator : AbstractValidator<CreateTableRequest>
{
    public CreateTableRequestValidator()
    {
        RuleFor(x => x.TableNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("Capacity must be greater than 0");
        RuleFor(x => x.Seats).GreaterThan(0).WithMessage("Seats must be greater than 0");
        RuleFor(x => x.Section).MaximumLength(50);
    }
}
