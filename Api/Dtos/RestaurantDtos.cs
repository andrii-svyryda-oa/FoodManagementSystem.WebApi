using Domain.Restaurants;

namespace Api.Dtos;

public record RestaurantDto(
    Guid? Id,
    string Name,
    string Description)
{
    public static RestaurantDto FromDomainModel(Restaurant restaurant)
        => new(
            Id: restaurant.Id.Value,
            Name: restaurant.Name,
            Description: restaurant.Description);
}
