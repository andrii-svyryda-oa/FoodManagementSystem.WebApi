using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public class ChangeUserBalanceCommandHandler(IUserRepository userRepository)
    : IRequestHandler<ChangeUserBalanceCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(ChangeUserBalanceCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var user = await userRepository.GetById(userId, cancellationToken);

        return await user.Match(
            async u => {
                var difference = request.Difference;

                return await UpdateEntity(u, difference, cancellationToken);
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User entity,
        decimal difference,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateBalance(difference);

            return await userRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}
