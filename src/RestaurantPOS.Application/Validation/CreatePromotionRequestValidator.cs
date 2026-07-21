using FluentValidation;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Validation;

public class CreatePromotionRequestValidator : AbstractValidator<CreatePromotionRequest>
{
    public CreatePromotionRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100).When(x => x.IsPercentBased);
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0).When(x => !x.IsPercentBased);
        RuleFor(x => x.ValidUntil).GreaterThan(x => x.ValidFrom);
        RuleFor(x => x.MaxUses).GreaterThan(0).When(x => x.MaxUses.HasValue);
    }
}
