using FluentValidation;

namespace Application.Orders.Commands;

public class CloseOrderCommandValidator : AbstractValidator<CloseOrderCommand>
{
    public CloseOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty();
    }
}
