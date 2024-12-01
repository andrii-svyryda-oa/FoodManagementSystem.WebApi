using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Security;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record CreateUserCommand : IRequest<Result<User, UserException>>
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required UserRole Role { get; init; }
}

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : IRequestHandler<CreateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmail(
            request.Email,
            cancellationToken);

        return await existingUser.Match(
            u => Task.FromResult<Result<User, UserException>>(new UserAlreadyExistsException(u.Id)),
            async () => await CreateEntity(request.Name, request.Email, passwordHasher.HashPassword(request.Password), request.Role, cancellationToken));
    }

    private async Task<Result<User, UserException>> CreateEntity(
        string name,
        string email,
        string password,
        UserRole role,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = User.New(UserId.New(), name, email, password, role);

            return await userRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(UserId.Empty(), exception);
        }
    }
}
