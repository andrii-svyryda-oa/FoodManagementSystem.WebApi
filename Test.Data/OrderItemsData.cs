using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;
namespace Tests.Common;

public static class OrderItemsData
{
    public static OrderItem MainOrderItem(OrderId orderId, UserId userId)
        => OrderItem.New(OrderItemId.New(), "КОМПЛЕКС 1", 150, userId, orderId);

    public static OrderItem SecondaryOrderItem(OrderId orderId, UserId userId, decimal price)
        => OrderItem.New(OrderItemId.New(), "SECONDARY КОМПЛЕКС 1", price, userId, orderId);
}
