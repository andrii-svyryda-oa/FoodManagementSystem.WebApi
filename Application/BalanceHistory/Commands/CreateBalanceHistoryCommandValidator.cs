using FluentValidation;

namespace Application.BalanceHistories.Commands
{
    public class CreateBalanceHistoryCommandValidator : AbstractValidator<CreateBalanceHistoryCommand>
    {
        public CreateBalanceHistoryCommandValidator()
        {
            RuleFor(x => x.Details).NotEmpty().MaximumLength(255).MinimumLength(3);
            RuleFor(x => x.Difference).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
