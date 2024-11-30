using FluentValidation;

namespace Application.Restaurants.Commands;

public class UpdateRestaurantCommandValidator : AbstractValidator<UpdateRestaurantCommand>
{
    public UpdateRestaurantCommandValidator()
    {
        RuleFor(c => c.RestaurantId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
