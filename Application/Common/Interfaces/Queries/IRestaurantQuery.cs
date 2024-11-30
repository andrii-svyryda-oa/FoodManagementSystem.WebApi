using Domain.Restaurants;

namespace Application.Common.Interfaces.Queries;

public interface IRestaurantQueries
{
    Task<IReadOnlyList<Restaurant>> GetAll(CancellationToken cancellationToken);
}
