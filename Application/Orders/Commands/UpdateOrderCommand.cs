using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Orders.Exceptions;
using Domain.OrderItems;
using Domain.Orders;
using MediatR;

namespace Application.Orders.Commands;

public record UpdateOrderCommand : IRequest<Result<Order, OrderException>>
{
    public required Guid OrderId { get; init; }
    public required string Name { get; init; }
    public required OrderState State { get; init; }
}

public class UpdateOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<UpdateOrderCommand, Result<Order, OrderException>>
{
    public async Task<Result<Order, OrderException>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderId = new OrderId(request.OrderId);

        var existingOrder = await orderRepository.GetById(orderId, cancellationToken);

        return await existingOrder.Match(
            async o => await UpdateEntity(o, request.Name, request.State, cancellationToken),
            () => Task.FromResult<Result<Order, OrderException>>(new OrderNotFoundException(orderId)));
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
