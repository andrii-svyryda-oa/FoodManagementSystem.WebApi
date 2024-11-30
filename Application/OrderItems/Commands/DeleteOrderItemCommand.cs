using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Domain.OrderItems;
using MediatR;

namespace Application.OrderItems.Commands;

public record DeleteOrderItemCommand : IRequest<Result<OrderItem, OrderItemException>>
{
    public required Guid OrderItemId { get; init; }
}

public class DeleteOrderItemCommandHandler(IOrderItemRepository orderItemRepository)
    : IRequestHandler<DeleteOrderItemCommand, Result<OrderItem, OrderItemException>>
{
    public async Task<Result<OrderItem, OrderItemException>> Handle(
        DeleteOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        var orderItemId = new OrderItemId(request.OrderItemId);

        var existingOrderItem = await orderItemRepository.GetById(orderItemId, cancellationToken);

        return await existingOrderItem.Match<Task<Result<OrderItem, OrderItemException>>>(
            async oi => await DeleteEntity(oi, cancellationToken),
            () => Task.FromResult<Result<OrderItem, OrderItemException>>(new OrderItemNotFoundException(orderItemId)));
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
