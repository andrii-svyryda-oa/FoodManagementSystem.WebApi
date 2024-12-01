using Domain.BalanceHistories;
using Domain.Users;

namespace Tests.Common;

public static class BalanceHistoryData
{
    public static BalanceHistory MainBalanceHistory(UserId userId)
        => BalanceHistory.New(BalanceHistoryId.New(), userId, "Поповнення балансу", 1000);
}
