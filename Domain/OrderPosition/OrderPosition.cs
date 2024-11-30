using Domain.Users;

namespace Domain.OrderPositions;

public class OrderPosition
{
    public OrderPositionId Id { get; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public UserId UserId { get; }
    public DateTime CreatedAt { get; private set; }

    private OrderPosition(OrderPositionId id, string name, decimal price, UserId userId, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Price = price;
        UserId = userId;
        CreatedAt = createdAt;
    }

    public static OrderPosition New(OrderPositionId id, string name, decimal price, UserId userId)
        => new(id, name, price, userId, DateTime.UtcNow);
}
