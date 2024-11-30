using FluentValidation;

namespace Application.OrderItems.Commands;

public class UpdateOrderItemCommandValidator : AbstractValidator<UpdateOrderItemCommand>
{
    public UpdateOrderItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.OrderItemId).NotEmpty();
    }
}
