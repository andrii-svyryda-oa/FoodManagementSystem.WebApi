using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Restaurants.Exceptions;
using Domain.Restaurants;
using MediatR;

namespace Application.Restaurants.Commands;

public record UpdateRestaurantCommand : IRequest<Result<Restaurant, RestaurantException>>
{
    public required Guid RestaurantId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public class UpdateRestaurantCommandHandler(IRestaurantRepository restaurantRepository)
    : IRequestHandler<UpdateRestaurantCommand, Result<Restaurant, RestaurantException>>
{
    public async Task<Result<Restaurant, RestaurantException>> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurantId = new RestaurantId(request.RestaurantId);

        var existingRestaurant = await restaurantRepository.GetById(restaurantId, cancellationToken);

        return await existingRestaurant.Match(
            async r => await UpdateEntity(r, request.Name, request.Description, cancellationToken),
            () => Task.FromResult<Result<Restaurant, RestaurantException>>(new RestaurantNotFoundException(restaurantId)));
    }

    private async Task<Result<Restaurant, RestaurantException>> UpdateEntity(
        Restaurant entity,
        string name,
        string description,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(name, description);

            return await restaurantRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new RestaurantUnknownException(entity.Id, exception);
        }
    }
}
