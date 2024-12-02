using Domain.Users;

namespace Domain.BalanceHistories;

public class BalanceHistory
{
    public BalanceHistoryId Id { get; }
    public UserId UserId { get; }
    public string Details { get; private set; }
    public decimal Difference { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private BalanceHistory(BalanceHistoryId id, UserId userId, string details, decimal difference, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        Details = details;
        Difference = difference;
        CreatedAt = createdAt;
    }

    public static BalanceHistory New(BalanceHistoryId id, UserId userId, string reason, decimal difference)
        => new(id, userId, reason, difference, DateTime.UtcNow);

    public void SubstractFromDifference(decimal difference)
    {
        Difference -= difference;
    }
}
