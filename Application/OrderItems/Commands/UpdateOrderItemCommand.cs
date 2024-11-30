using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Domain.OrderItems;
using MediatR;

namespace Application.OrderItems.Commands;

public record UpdateOrderItemCommand : IRequest<Result<OrderItem, OrderItemException>>
{
    public required Guid OrderItemId { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
}

public class UpdateOrderItemCommandHandler(IOrderItemRepository orderItemRepository)
    : IRequestHandler<UpdateOrderItemCommand, Result<OrderItem, OrderItemException>>
{
    public async Task<Result<OrderItem, OrderItemException>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {
        var orderItemId = new OrderItemId(request.OrderItemId);

        var existingOrderItem = await orderItemRepository.GetById(orderItemId, cancellationToken);

        return await existingOrderItem.Match(
            async oi => await UpdateEntity(oi, request.Name, request.Price, cancellationToken),
            () => Task.FromResult<Result<OrderItem, OrderItemException>>(new OrderItemNotFoundException(orderItemId)));
    }

    private async Task<Result<OrderItem, OrderItemException>> UpdateEntity(
        OrderItem entity,
        string name,
        decimal price,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(name, price);

            return await orderItemRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new OrderItemUnknownException(entity.Id, exception);
        }
    }
}
