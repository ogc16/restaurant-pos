using FluentValidation;
using RestaurantPOS.Core.DTOs;

namespace RestaurantPOS.Application.Validation;

public class UpdatePromotionRequestValidator : AbstractValidator<UpdatePromotionRequest>
{
    public UpdatePromotionRequestValidator()
    {
        RuleFor(x => x.Code).MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.DiscountPercent).InclusiveBetween(0, 100).When(x => x.DiscountPercent.HasValue);
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0).When(x => x.DiscountAmount.HasValue);
        RuleFor(x => x.ValidUntil).GreaterThan(x => x.ValidFrom).When(x => x.ValidFrom.HasValue && x.ValidUntil.HasValue);
        RuleFor(x => x.MaxUses).GreaterThan(0).When(x => x.MaxUses.HasValue);
    }
}
