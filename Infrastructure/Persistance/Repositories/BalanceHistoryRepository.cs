using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.BalanceHistories;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class BalanceHistoryRepository(ApplicationDbContext context) : IBalanceHistoryRepository, IBalanceHistoryQueries
{
    public async Task<IReadOnlyList<BalanceHistory>> GetByUserId(UserId userId, CancellationToken cancellationToken)
    {
        return await context.BalanceHistory
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<BalanceHistory> Add(BalanceHistory balanceHistory, CancellationToken cancellationToken)
    {
        await context.BalanceHistory.AddAsync(balanceHistory, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return balanceHistory;
    }

    public async Task<List<BalanceHistory>> AddMany(List<BalanceHistory> balanceHistories, CancellationToken cancellationToken)
    {
        await context.BalanceHistory.AddRangeAsync(balanceHistories);
        await context.SaveChangesAsync(cancellationToken);
        return balanceHistories;
    }
}
