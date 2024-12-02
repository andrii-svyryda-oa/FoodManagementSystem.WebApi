using Domain.BalanceHistories;

namespace Application.Common.Interfaces.Repositories;

public interface IBalanceHistoryRepository
{
    Task<BalanceHistory> Add(BalanceHistory balanceHistory, CancellationToken cancellationToken);
    Task<List<BalanceHistory>> AddMany(List<BalanceHistory> balanceHistory, CancellationToken cancellationToken);
}
