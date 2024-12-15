using Domain.Restaurants;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IRestaurantRepository
{
    Task<Restaurant> Add(Restaurant restaurant, CancellationToken cancellationToken);
    Task<Restaurant> Update(Restaurant restaurant, CancellationToken cancellationToken);
    Task<Restaurant> Delete(Restaurant restaurant, CancellationToken cancellationToken);
    Task<Option<Restaurant>> GetById(RestaurantId id, CancellationToken cancellationToken);
    Task<Option<Restaurant>> GetByName(string name, CancellationToken cancellationToken);
}
