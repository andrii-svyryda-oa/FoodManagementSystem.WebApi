using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Restaurants;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class RestaurantRepository(ApplicationDbContext context) : IRestaurantRepository, IRestaurantQueries
{
    public async Task<Option<Restaurant>> GetById(RestaurantId id, CancellationToken cancellationToken)
    {
        var entity = await context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Restaurant>() : Option.Some(entity);
    }

    public async Task<Option<Restaurant>> GetByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<Restaurant>() : Option.Some(entity);
    }

    public async Task<(IReadOnlyList<Restaurant>, int)> GetAll(int skip, int take, string searchText, CancellationToken cancellationToken)
    {
        var query = context.Restaurants.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var pattern = $"%{searchText.ToLower()}%";

            query = query.Where(
                x => EF.Functions.Like(x.Name.ToLower(), pattern) &&
                EF.Functions.Like(x.Description.ToLower(), pattern));
        }

        var count = await query.CountAsync(cancellationToken);
        var result = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);

        return (result, count);
    }

    public async Task<Restaurant> Add(Restaurant restaurant, CancellationToken cancellationToken)
    {
        await context.Restaurants.AddAsync(restaurant, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return restaurant;
    }

    public async Task<Restaurant> Update(Restaurant restaurant, CancellationToken cancellationToken)
    {
        context.Restaurants.Update(restaurant);
        await context.SaveChangesAsync(cancellationToken);
        return restaurant;
    }

    public async Task<Restaurant> Delete(Restaurant restaurant, CancellationToken cancellationToken)
    {
        context.Restaurants.Remove(restaurant);
        await context.SaveChangesAsync(cancellationToken);
        return restaurant;
    }
}
