using Domain.Orders;
using Domain.Restaurants;
using Domain.Users;

namespace Tests.Common;

public static class OrdersData
{
    public static Order MainOrder(RestaurantId restaurantId, UserId userId)
        => Order.New(OrderId.New(), userId, "обід", restaurantId);
}
