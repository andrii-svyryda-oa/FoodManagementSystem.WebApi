using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Domain.OrderItems;
using Domain.Users;
using MediatR;

namespace Application.OrderItems.Commands;

public record DeleteOrderItemCommand : IRequest<Result<OrderItem, OrderItemException>>
{
    public required Guid OrderItemId { get; init; }
    public required Guid UserId { get; init; }
}

public class DeleteOrderItemCommandHandler(
    IOrderItemRepository orderItemRepository,
    IUserRepository userRepository)
    : OrderItemCommandHandlerBase(userRepository, orderItemRepository), IRequestHandler<DeleteOrderItemCommand, Result<OrderItem, OrderItemException>>
{
    public async Task<Result<OrderItem, OrderItemException>> Handle(
        DeleteOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        var orderItemId = new OrderItemId(request.OrderItemId);
        var authorId = new UserId(request.UserId);

        var existingOrderItem = await ReadDbOrderItem(orderItemId, authorId, cancellationToken);

        return await existingOrderItem.MatchAsync(
            async oi => await DeleteEntity(oi, cancellationToken),
            e => e);
    }

    public async Task<Result<OrderItem, OrderItemException>> DeleteEntity(OrderItem orderItem, CancellationToken cancellationToken)
    {
        try
        {
            return await orderItemRepository.Delete(orderItem, cancellationToken);
        }
        catch (Exception exception)
        {
            return new OrderItemUnknownException(orderItem.Id, exception);
        }
    }
}
