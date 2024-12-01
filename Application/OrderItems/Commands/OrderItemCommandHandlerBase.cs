using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Exceptions;
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
                var author = await userRepository.GetById(authorId, cancellationToken);

                return author.Match<Result<OrderItem, OrderItemException>>(
                    author => {
                        if (author.Role == UserRole.Admin)
                        {
                            return orderItem;
                        }

                        if (author.Id == orderItem.UserId)
                        {
                            return orderItem;
                        }

                        return new OrderItemOperationForbiddenException(orderItemId);
                    },
                    () => new OrderItemAuthorNotFoundException(authorId));
            },
            () => Task.FromResult<Result<OrderItem, OrderItemException>>(new OrderItemNotFoundException(orderItemId)));
    }
}
