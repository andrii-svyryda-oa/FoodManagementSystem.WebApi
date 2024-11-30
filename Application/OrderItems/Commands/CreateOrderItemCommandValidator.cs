using FluentValidation;

namespace Application.OrderItems.Commands;

public class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemCommand>
{
    public CreateOrderItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
