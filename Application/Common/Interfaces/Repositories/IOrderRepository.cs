using Domain.Orders;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order> Add(Order order, CancellationToken cancellationToken);
    Task<Order> Update(Order order, CancellationToken cancellationToken);
    Task<Option<Order>> GetById(OrderId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetByUserId(UserId userId, CancellationToken cancellationToken);
    Task<Order> Delete(Order order, CancellationToken cancellationToken);
}
