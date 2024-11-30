using FluentValidation;

namespace Application.Orders.Commands;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(30);
    }
}
