using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class UserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public UserDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class AdjustUserBalanceDtoValidator : AbstractValidator<AdjustUserBalanceDto>
{
    public AdjustUserBalanceDtoValidator()
    {
        RuleFor(x => x.Details).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Difference).NotEmpty();
    }
}
