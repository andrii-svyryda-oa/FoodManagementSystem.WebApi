using Domain.BalanceHistories;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IBalanceHistoryQueries
{
    Task<IReadOnlyList<BalanceHistory>> GetByUserId(UserId userId, CancellationToken cancellationToken);
}
