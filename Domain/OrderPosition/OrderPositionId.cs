namespace Domain.OrderPositions;

public record OrderPositionId(Guid Value)
{
    public static OrderPositionId New() => new(Guid.NewGuid());
    public static OrderPositionId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}
