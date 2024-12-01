using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Domain.OrderItems;
using Domain.Users;
using MediatR;

namespace Application.OrderItems.Commands;

public record UpdateOrderItemCommand : IRequest<Result<OrderItem, OrderItemException>>
{
    public required Guid OrderItemId { get; init; }
    public required Guid UserId { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
}

public class UpdateOrderItemCommandHandler(
    IOrderItemRepository orderItemRepository,
    IUserRepository userRepository
    ) : OrderItemCommandHandlerBase(userRepository, orderItemRepository), IRequestHandler<UpdateOrderItemCommand, Result<OrderItem, OrderItemException>>
{
    public async Task<Result<OrderItem, OrderItemException>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {
        var orderItemId = new OrderItemId(request.OrderItemId);
        var userId = new UserId(request.UserId);

        var existingOrderItem = await ReadDbOrderItem(orderItemId, userId, cancellationToken);

        return await existingOrderItem.MatchAsync(
            async oi => await UpdateEntity(oi, request.Name, request.Price, cancellationToken),
            e => e);
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
