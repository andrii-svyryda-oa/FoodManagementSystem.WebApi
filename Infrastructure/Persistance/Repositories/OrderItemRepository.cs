using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.OrderItems;
using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class OrderItemRepository(ApplicationDbContext context) : IOrderItemRepository, IOrderItemQueries
{
    public async Task<IReadOnlyList<OrderItem>> GetByOrderId(OrderId orderId, CancellationToken cancellationToken)
    {
        return await context.OrderItems
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<OrderItem>> GetById(OrderItemId id, CancellationToken cancellationToken)
    {
        var entity = await context.OrderItems
            .Include(x => x.Order)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<OrderItem>() : Option.Some(entity);
    }

    public async Task<OrderItem> Add(OrderItem orderItem, CancellationToken cancellationToken)
    {
        await context.OrderItems.AddAsync(orderItem, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return orderItem;
    }

    public async Task<OrderItem> Update(OrderItem orderItem, CancellationToken cancellationToken)
    {
        context.OrderItems.Update(orderItem);
        await context.SaveChangesAsync(cancellationToken);

        return orderItem;
    }

    public async Task<OrderItem> Delete(OrderItem orderItem, CancellationToken cancellationToken)
    {
        context.OrderItems.Remove(orderItem);
        await context.SaveChangesAsync(cancellationToken);

        return orderItem;
    }
}
