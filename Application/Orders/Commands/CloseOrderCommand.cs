using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Commands;
using Application.Orders.Exceptions;
using Domain.BalanceHistories;
using Domain.Orders;
using Domain.Users;
using MediatR;
using Optional.Collections;

namespace Application.Orders.Commands;

public record CloseOrderCommand : IRequest<Result<Order, OrderException>>
{
    public required Guid OrderId { get; init; }
    public required Guid AuthorId { get; init; }
}

public class CloseOrderCommandHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IBalanceHistoryRepository balanceHistoryRepository)
    : OrderCommandHandlerBase(userRepository, orderRepository), IRequestHandler<CloseOrderCommand, Result<Order, OrderException>>
{
    public async Task<Result<Order, OrderException>> Handle(CloseOrderCommand request, CancellationToken cancellationToken)
    {
        var orderId = new OrderId(request.OrderId);
        var userId = new UserId(request.AuthorId);

        var existingOrder = await ReadDbOrder(orderId, userId, cancellationToken);

        return await existingOrder.BindAsync(
            async o => {
                var result = await UpdateEntity(o, OrderState.Closed, cancellationToken);

                return await result.BindAsync(
                    async o =>
                    {
                        return await UpdateUsersBalances(o, cancellationToken);
                    });
            });
    }

    private async Task<Result<Order, OrderException>> UpdateUsersBalances(
        Order entity, 
        CancellationToken cancellationToken)
    {
        try
        {
            var userBalanceHistoryItemDict = CalculateDifferences(entity);

            var balanceHistories = userBalanceHistoryItemDict.Values.ToList();

            await balanceHistoryRepository.AddMany(balanceHistories, cancellationToken);

            var users = await userRepository.GetByIds(balanceHistories.Select(x => x.UserId).ToList(), cancellationToken);

            foreach (var user in users)
            {
                userBalanceHistoryItemDict.GetValueOrNone(user.Id).MatchSome(
                    balanceHistory => user.UpdateBalance(balanceHistory.Difference));
            }

            await userRepository.UpdateMany(users, cancellationToken);

            return entity;
        }
        catch (Exception exception)
        {
            return new OrderUnknownException(entity.Id, exception);
        }
    }

    private Dictionary<UserId, BalanceHistory> CalculateDifferences(Order order)
    {
        var userBalanceHistoryItemDict = new Dictionary<UserId, BalanceHistory>();

        var billName = order.CloseOrderBillName;

        foreach (var item in order.Items!)
        {
            if (userBalanceHistoryItemDict.ContainsKey(item.UserId))
            {
                userBalanceHistoryItemDict[item.UserId].SubstractFromDifference(item.Price);

                continue;
            }

            var newBalanceHistory = BalanceHistory.New(BalanceHistoryId.New(), item.UserId, billName, -item.Price);

            userBalanceHistoryItemDict.Add(item.UserId, newBalanceHistory);
        }

        return userBalanceHistoryItemDict;
    }

    private async Task<Result<Order, OrderException>> UpdateEntity(
        Order entity,
        OrderState state,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(entity.Name, state);

            return await orderRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new OrderUnknownException(entity.Id, exception);
        }
    }
}
