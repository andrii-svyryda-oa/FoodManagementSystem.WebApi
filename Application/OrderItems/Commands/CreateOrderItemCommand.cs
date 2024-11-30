using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;
using MediatR;

namespace Application.OrderItems.Commands;

public record CreateOrderItemCommand : IRequest<Result<OrderItem, OrderItemException>>
{
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required Guid UserId { get; init; }
    public required Guid OrderId { get; init; }
}

public class CreateOrderItemCommandHandler(
    IOrderItemRepository orderItemRepository,
    IOrderRepository orderRepository,
    IUserRepository userRepository)
    : IRequestHandler<CreateOrderItemCommand, Result<OrderItem, OrderItemException>>
{
    public async Task<Result<OrderItem, OrderItemException>> Handle(CreateOrderItemCommand request, CancellationToken cancellationToken)
    {
        var orderId = new OrderId(request.OrderId);

        var order = await orderRepository.GetById(orderId, cancellationToken);

        return await order.Match<Task<Result<OrderItem, OrderItemException>>>(
            async o =>
            {
                var userId = new UserId(request.UserId);

                var user = await userRepository.GetById(userId, cancellationToken);

                return await user.Match<Task<Result<OrderItem, OrderItemException>>>(
                    async u => await CreateEntity(request.Name, request.Price, userId, orderId, cancellationToken),
                    () => Task.FromResult<Result<OrderItem, OrderItemException>>(new OrderItemUserNotFoundException(userId)));
            },
            () => Task.FromResult<Result<OrderItem, OrderItemException>>(new OrderItemOrderNotFoundException(orderId)));
    }

    private async Task<Result<OrderItem, OrderItemException>> CreateEntity(
        string name,
        decimal price,
        UserId userId,
        OrderId orderId,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = OrderItem.New(OrderItemId.New(), name, price, userId, orderId);

            return await orderItemRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new OrderItemUnknownException(OrderItemId.Empty(), exception);
        }
    }
}
