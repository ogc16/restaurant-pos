using FluentValidation;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Validation;

public class UpdateTableRequestValidator : AbstractValidator<UpdateTableRequest>
{
    public UpdateTableRequestValidator()
    {
        RuleFor(x => x.TableNumber).MaximumLength(20);
        RuleFor(x => x.Capacity).GreaterThan(0).When(x => x.Capacity.HasValue);
        RuleFor(x => x.Seats).GreaterThan(0).When(x => x.Seats.HasValue);
        RuleFor(x => x.Section).MaximumLength(50);
    }
}
