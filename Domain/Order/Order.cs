using Domain.Users;

namespace Domain.Orders;

public class Order
{
    public OrderId Id { get; }
    public UserId OwnerId { get; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid RestaurantId { get; private set; }
    public string State { get; private set; } // Opened | Closed | Cancelled

    private Order(OrderId id, UserId ownerId, string name, Guid restaurantId, string state, DateTime createdAt)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        RestaurantId = restaurantId;
        State = state;
        CreatedAt = createdAt;
    }

    public static Order New(OrderId id, UserId ownerId, string name, Guid restaurantId, string state)
        => new(id, ownerId, name, restaurantId, state, DateTime.UtcNow);

    public void UpdateState(string state)
    {
        State = state;
    }
}
