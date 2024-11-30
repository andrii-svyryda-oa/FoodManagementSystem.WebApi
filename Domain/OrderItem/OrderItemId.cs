namespace Domain.OrderItems;

public record OrderItemId(Guid Value)
{
    public static OrderItemId New() => new(Guid.NewGuid());
    public static OrderItemId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}
