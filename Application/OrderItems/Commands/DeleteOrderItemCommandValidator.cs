using FluentValidation;

namespace Application.OrderItems.Commands;

public class DeleteOrderItemCommandValidator : AbstractValidator<DeleteOrderItemCommand>
{
    public DeleteOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderItemId).NotEmpty();
    }
}
