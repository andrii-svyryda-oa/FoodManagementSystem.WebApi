using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(2);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
