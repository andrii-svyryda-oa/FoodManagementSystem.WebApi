using Application.BalanceHistories.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.BalanceHistories;
using Domain.Users;
using MediatR;

namespace Application.BalanceHistories.Commands;

public record CreateBalanceHistoryCommand : IRequest<Result<BalanceHistory, BalanceHistoryException>>
{
    public required string Details { get; init; }
    public required decimal Difference { get; init; }
    public required Guid UserId { get; init; }
}

public class CreateBalanceHistoryCommandHandler(
    IBalanceHistoryRepository balanceHistoryRepository,
    IUserRepository userRepository)
    : IRequestHandler<CreateBalanceHistoryCommand, Result<BalanceHistory, BalanceHistoryException>>
{
    public async Task<Result<BalanceHistory, BalanceHistoryException>> Handle(CreateBalanceHistoryCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var user = await userRepository.GetById(userId, cancellationToken);

        return await user.Match<Task<Result<BalanceHistory, BalanceHistoryException>>>(
            async c => await CreateEntity(userId, request.Details, request.Difference, cancellationToken),
            () => Task.FromResult<Result<BalanceHistory, BalanceHistoryException>>(new BalanceHistoryUserNotFoundException(userId)));
    }

    private async Task<Result<BalanceHistory, BalanceHistoryException>> CreateEntity(
        UserId userId,
        string details,
        decimal difference,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = BalanceHistory.New(BalanceHistoryId.New(), userId, details, difference);

            return await balanceHistoryRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new BalanceHistoryUnknownException(BalanceHistoryId.Empty(), exception);
        }
    }
}


