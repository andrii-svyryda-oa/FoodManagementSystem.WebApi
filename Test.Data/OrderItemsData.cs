using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;
namespace Tests.Common;

public static class OrderItemsData
{
    public static OrderItem MainOrderItem(OrderId orderId, UserId userId)
        => OrderItem.New(OrderItemId.New(), "КОМПЛЕКС 1", 150, userId, orderId);
}
