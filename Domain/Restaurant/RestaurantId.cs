namespace Domain.Restaurants;

public record RestaurantId(Guid Value)
{
    public static RestaurantId New() => new(Guid.NewGuid());
    public static RestaurantId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}
