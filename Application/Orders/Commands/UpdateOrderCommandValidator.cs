using FluentValidation;

namespace Application.Orders.Commands;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(30);
    }
}
