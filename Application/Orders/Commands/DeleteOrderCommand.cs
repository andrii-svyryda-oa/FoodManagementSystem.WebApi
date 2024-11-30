using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Orders.Exceptions;
using Domain.Orders;
using MediatR;

namespace Application.Orders.Commands;

public record DeleteOrderCommand : IRequest<Result<Order, OrderException>>
{
    public required Guid OrderId { get; init; }
}

public class DeleteOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<DeleteOrderCommand, Result<Order, OrderException>>
{
    public async Task<Result<Order, OrderException>> Handle(
        DeleteOrderCommand request,
        CancellationToken cancellationToken)
    {
        var orderId = new OrderId(request.OrderId);

        var existingOrder = await orderRepository.GetById(orderId, cancellationToken);

        return await existingOrder.Match<Task<Result<Order, OrderException>>>(
            async o => await DeleteEntity(o, cancellationToken),
            () => Task.FromResult<Result<Order, OrderException>>(new OrderNotFoundException(orderId)));
    }

    public async Task<Result<Order, OrderException>> DeleteEntity(Order order, CancellationToken cancellationToken)
    {
        try
        {
            return await orderRepository.Delete(order, cancellationToken);
        }
        catch (Exception exception)
        {
            return new OrderUnknownException(order.Id, exception);
        }
    }
}
