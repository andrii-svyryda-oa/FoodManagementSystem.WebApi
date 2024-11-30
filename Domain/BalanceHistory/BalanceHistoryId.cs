namespace Domain.BalanceHistories;

public record BalanceHistoryId(Guid Value)
{
    public static BalanceHistoryId New() => new(Guid.NewGuid());
    public static BalanceHistoryId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}
