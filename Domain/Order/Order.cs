using Domain.OrderItems;
using Domain.Restaurants;
using Domain.Users;

namespace Domain.Orders;

public class Order
{
    public OrderId Id { get; }
    public UserId OwnerId { get; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public RestaurantId RestaurantId { get; private set; }
    public Restaurant? Restaurant { get; private set; }
    public OrderState State { get; private set; }
    public List<OrderItem>? Items { get; private set; }

    private Order(OrderId id, UserId ownerId, string name, RestaurantId restaurantId, OrderState state, DateTime createdAt)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        RestaurantId = restaurantId;
        State = state;
        CreatedAt = createdAt;
    }

    public static Order New(OrderId id, UserId ownerId, string name, RestaurantId restaurantId, OrderState state)
        => new(id, ownerId, name, restaurantId, state, DateTime.UtcNow);

    public void UpdateState(OrderState state)
    {
        State = state;
    }
}
