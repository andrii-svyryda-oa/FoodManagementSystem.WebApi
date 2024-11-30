using FluentValidation;

namespace Application.Restaurants.Commands;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
