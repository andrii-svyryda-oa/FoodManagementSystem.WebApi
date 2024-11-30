using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Restaurants.Exceptions;
using Application.Security;
using Application.Users.Exceptions;
using Domain.Restaurants;
using Domain.Users;
using MediatR;

namespace Application.Restaurants.Commands;

public record CreateRestaurantCommand : IRequest<Result<Restaurant, RestaurantException>>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public class CreateRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository)
    : IRequestHandler<CreateRestaurantCommand, Result<Restaurant, RestaurantException>>
{
    public async Task<Result<Restaurant, RestaurantException>> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var existingRestaurant = await restaurantRepository.GetByName(request.Name, cancellationToken);

        return await existingRestaurant.Match<Task<Result<Restaurant, RestaurantException>>>(
            r => Task.FromResult<Result<Restaurant, RestaurantException>>(new RestaurantAlreadyExistsException(r.Id)),
            async () => await CreateEntity(request.Name, request.Description, cancellationToken));
    }

    private async Task<Result<Restaurant, RestaurantException>> CreateEntity(
        string name,
        string description,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Restaurant.New(RestaurantId.New(), name, description);

            return await restaurantRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new RestaurantUnknownException(RestaurantId.Empty(), exception);
        }
    }
}
