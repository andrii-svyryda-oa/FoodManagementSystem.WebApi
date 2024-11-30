using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Restaurants.Exceptions;
using Domain.Restaurants;
using MediatR;

namespace Application.Restaurants.Commands;

public record DeleteRestaurantCommand : IRequest<Result<Restaurant, RestaurantException>>
{
    public required Guid RestaurantId { get; init; }
}

public class DeleteRestaurantCommandHandler(IRestaurantRepository restaurantRepository)
    : IRequestHandler<DeleteRestaurantCommand, Result<Restaurant, RestaurantException>>
{
    public async Task<Result<Restaurant, RestaurantException>> Handle(
        DeleteRestaurantCommand request,
        CancellationToken cancellationToken)
    {
        var restaurantId = new RestaurantId(request.RestaurantId);

        var existingRestaurant = await restaurantRepository.GetById(restaurantId, cancellationToken);

        return await existingRestaurant.Match<Task<Result<Restaurant, RestaurantException>>>(
            async r => await DeleteEntity(r, cancellationToken),
            () => Task.FromResult<Result<Restaurant, RestaurantException>>(new RestaurantNotFoundException(restaurantId)));
    }

    public async Task<Result<Restaurant, RestaurantException>> DeleteEntity(Restaurant restaurant, CancellationToken cancellationToken)
    {
        try
        {
            return await restaurantRepository.Delete(restaurant, cancellationToken);
        }
        catch (Exception exception)
        {
            return new RestaurantUnknownException(restaurant.Id, exception);
        }
    }
}
