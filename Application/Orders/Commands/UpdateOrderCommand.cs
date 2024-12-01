using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Commands;
using Application.Orders.Exceptions;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;
using MediatR;

namespace Application.Orders.Commands;

public record UpdateOrderCommand : IRequest<Result<Order, OrderException>>
{
    public required Guid OrderId { get; init; }
    public required Guid AuthorId { get; init; }
    public required string Name { get; init; }
    public required OrderState State { get; init; }
}

public class UpdateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository)
    : OrderCommandHandlerBase(userRepository, orderRepository), IRequestHandler<UpdateOrderCommand, Result<Order, OrderException>>
{
    public async Task<Result<Order, OrderException>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderId = new OrderId(request.OrderId);
        var userId = new UserId(request.AuthorId);

        var existingOrder = await ReadDbOrder(orderId, userId, cancellationToken);

        return await existingOrder.MatchAsync(
            async o => await UpdateEntity(o, request.Name, request.State, cancellationToken),
            e => e);
    }

    private async Task<Result<Order, OrderException>> UpdateEntity(
        Order entity,
        string name,
        OrderState state,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(name, state);

            return await orderRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new OrderUnknownException(entity.Id, exception);
        }
    }
}
