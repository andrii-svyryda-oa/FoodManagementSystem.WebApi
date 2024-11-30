using FluentValidation;

namespace Application.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MaximumLength(20).MinimumLength(6);
        RuleFor(x => x.Role).NotEmpty();
    }
}
