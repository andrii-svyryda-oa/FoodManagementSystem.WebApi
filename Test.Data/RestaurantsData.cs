using Domain.Restaurants;

namespace Tests.Common;

public static class RestaurantsData
{
    public static Restaurant MainRestaurant()
        => Restaurant.New(RestaurantId.New(), "Lunch", "lunch.rv");
}
