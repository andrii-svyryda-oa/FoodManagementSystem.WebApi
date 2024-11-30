using FluentValidation;

namespace Application.Restaurants.Commands;

public class DeleteRestaurantCommandValidator : AbstractValidator<DeleteRestaurantCommand>
{
    public DeleteRestaurantCommandValidator()
    {
        RuleFor(c => c.RestaurantId).NotEmpty();
    }
}
