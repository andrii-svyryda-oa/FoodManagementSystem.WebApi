using Domain.OrderItems;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IOrderItemRepository
{
    Task<OrderItem> Add(OrderItem orderItem, CancellationToken cancellationToken);
    Task<OrderItem> Update(OrderItem orderItem, CancellationToken cancellationToken);
    Task<OrderItem> Delete(OrderItem orderItem, CancellationToken cancellationToken);
    Task<Option<OrderItem>> GetById(OrderItemId id, CancellationToken cancellationToken);
}
