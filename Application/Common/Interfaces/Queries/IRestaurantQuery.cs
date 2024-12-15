using Domain.Restaurants;

namespace Application.Common.Interfaces.Queries;

public interface IRestaurantQueries
{
    Task<(IReadOnlyList<Restaurant>, int)> GetAll(int skip, int take, string searchText, CancellationToken cancellationToken);
}
