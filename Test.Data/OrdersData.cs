using Domain.Orders;
using Domain.Restaurants;
using Domain.Users;

namespace Tests.Common;

public static class OrdersData
{
    public static Order MainOrder(RestaurantId restaurantId, UserId userId)
        => Order.New(OrderId.New(), userId, "обід", restaurantId);
    public static Order SecondaryOrder(RestaurantId restaurantId, UserId userId, string identifier)
        => Order.New(OrderId.New(), userId, "secondary обід " + identifier, restaurantId);
}
