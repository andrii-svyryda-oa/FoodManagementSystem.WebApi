using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Application.Orders.Exceptions;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Restaurants;
using Domain.Users;
using MediatR;

namespace Application.Orders.Commands;

public record CreateOrderCommand : IRequest<Result<Order, OrderException>>
{
    public required Guid OwnerId { get; init; }
    public required Guid RestaurantId { get; init; }
    public required string Name { get; init; }
}

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IRestaurantRepository restaurantRepository)
    : IRequestHandler<CreateOrderCommand, Result<Order, OrderException>>
{
    public async Task<Result<Order, OrderException>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var ownerId = new UserId(request.OwnerId);

        var owner = await userRepository.GetById(ownerId, cancellationToken);

        return await owner.Match<Task<Result<Order, OrderException>>>(
            async c =>
            {
                var restaurantId = new RestaurantId(request.RestaurantId);
                
                var restaurant = await restaurantRepository.GetById(restaurantId, cancellationToken);

                return await restaurant.Match(
                    async r => await CreateEntity(ownerId, restaurantId, request.Name, cancellationToken),
                    () => Task.FromResult<Result<Order, OrderException>>(new OrderRestaurantNotFoundException(restaurantId)));
            },
            () => Task.FromResult<Result<Order, OrderException>>(new OrderOwnerNotFoundException(ownerId)));
    }

    private async Task<Result<Order, OrderException>> CreateEntity(
        UserId ownerId,
        RestaurantId restaurantId,
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Order.New(OrderId.New(), ownerId, name, restaurantId);

            return await orderRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new OrderUnknownException(OrderId.Empty(), exception);
        }
    }
}
