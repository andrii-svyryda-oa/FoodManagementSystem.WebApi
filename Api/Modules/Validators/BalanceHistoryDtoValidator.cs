using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class BalanceHistoryDtoValidator : AbstractValidator<BalanceHistoryDto>
{
    public BalanceHistoryDtoValidator()
    {
        RuleFor(x => x.Details).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Difference).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
