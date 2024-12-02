using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
using Application.Orders.Exceptions;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;
using MediatR;

namespace Application.OrderItems.Commands;

public class OrderItemCommandHandlerBase(
    IUserRepository userRepository,
    IOrderItemRepository orderItemRepository)
{
    protected async Task<Result<OrderItem, OrderItemException>> ReadDbOrderItem(
        OrderItemId orderItemId,
        UserId authorId,
        CancellationToken cancellationToken)
    {
        var orderItem = await orderItemRepository.GetById(orderItemId, cancellationToken);

        return await orderItem.Match(
            async orderItem =>
            {
                var order = orderItem.Order!;
                var author = await userRepository.GetById(authorId, cancellationToken);

                return author.Match<Result<OrderItem, OrderItemException>>(
                    author => {
                        var userHasAccess = author.Role == UserRole.Admin || author.Id == order.OwnerId;

                        if (!userHasAccess)
                        {
                            return new OrderItemOperationForbiddenException(orderItem.Id);
                        }

                        if (order.State == OrderState.Closed)
                        {
                            return new OrderItemOrderAlreadyClosedException(orderItem.Id);
                        }

                        return orderItem;
                    },
                    () => new OrderItemAuthorNotFoundException(authorId));
            },
            () => Task.FromResult<Result<OrderItem, OrderItemException>>(new OrderItemNotFoundException(orderItemId)));
    }
}
