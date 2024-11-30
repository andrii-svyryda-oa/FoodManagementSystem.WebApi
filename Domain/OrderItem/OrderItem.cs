using Domain.Orders;
using Domain.Users;

namespace Domain.OrderItems;

public class OrderItem
{
    public OrderItemId Id { get; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public UserId UserId { get; }
    public OrderId OrderId { get; }
    public DateTime CreatedAt { get; private set; }

    private OrderItem(OrderItemId id, string name, decimal price, UserId userId, OrderId orderId, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Price = price;
        UserId = userId;
        OrderId = orderId;
        CreatedAt = createdAt;
    }

    public static OrderItem New(OrderItemId id, string name, decimal price, UserId userId, OrderId orderId)
        => new(id, name, price, userId, orderId, DateTime.UtcNow);
}
