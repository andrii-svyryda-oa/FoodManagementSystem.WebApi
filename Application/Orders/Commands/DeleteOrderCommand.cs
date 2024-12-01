using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Commands;
using Application.Orders.Exceptions;
using Domain.Orders;
using Domain.Users;
using MediatR;

namespace Application.Orders.Commands;

public record DeleteOrderCommand : IRequest<Result<Order, OrderException>>
{
    public required Guid OrderId { get; init; }
    public required Guid AuthorId { get; init; }
}

public class DeleteOrderCommandHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository)
    : OrderCommandHandlerBase(userRepository, orderRepository), IRequestHandler<DeleteOrderCommand, Result<Order, OrderException>>
{
    public async Task<Result<Order, OrderException>> Handle(
        DeleteOrderCommand request,
        CancellationToken cancellationToken)
    {
        var orderId = new OrderId(request.OrderId);
        var authorId = new UserId(request.AuthorId);

        var existingOrder = await this.ReadDbOrder(orderId, authorId, cancellationToken);

        return await existingOrder.MatchAsync(
            async o => await DeleteEntity(o, cancellationToken),
            e => e);
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
