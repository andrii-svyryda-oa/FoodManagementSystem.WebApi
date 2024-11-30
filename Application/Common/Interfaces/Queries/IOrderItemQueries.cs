using Domain.OrderItems;
using Domain.Orders;

namespace Application.Common.Interfaces.Queries;

public interface IOrderItemQueries
{
    Task<IReadOnlyList<OrderItem>> GetByOrderId(OrderId orderId, CancellationToken cancellationToken);
}
