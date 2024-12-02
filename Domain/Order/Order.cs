using Domain.OrderItems;
using Domain.Restaurants;
using Domain.Users;

namespace Domain.Orders;

public class Order
{
    public OrderId Id { get; }
    public string Name { get; private set; }
    public UserId OwnerId { get; }
    public DateTime CreatedAt { get; private set; }
    public RestaurantId RestaurantId { get; private set; }
    public Restaurant? Restaurant { get; private set; }
    public OrderState State { get; private set; }
    public List<OrderItem>? Items { get; private set; }

    public string CloseOrderBillName { get => $"Bill for {Name} at {Restaurant!.Name}"; }

    private Order(OrderId id, UserId ownerId, string name, RestaurantId restaurantId, OrderState state, DateTime createdAt)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        RestaurantId = restaurantId;
        State = state;
        CreatedAt = createdAt;
    }

    public static Order New(OrderId id, UserId ownerId, string name, RestaurantId restaurantId)
        => new(id, ownerId, name, restaurantId, OrderState.Opened, DateTime.UtcNow);

    public void UpdateDetails(string name, OrderState state)
    {
        Name = name;
        State = state;
    }
}
