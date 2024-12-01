using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository, IOrderQueries
{
    public async Task<Option<Order>> GetById(OrderId id, CancellationToken cancellationToken)
    {
        var entity = await context.Orders
            .Include(x => x.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Order>() : Option.Some(entity);
    }

    public async Task<IReadOnlyList<Order>> GetByUserId(UserId userId, CancellationToken cancellationToken)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(x => x.OwnerId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Orders
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order> Add(Order order, CancellationToken cancellationToken)
    {
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<Order> Update(Order order, CancellationToken cancellationToken)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<Order> Delete(Order order, CancellationToken cancellationToken)
    {
        context.Orders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);

        return order;
    }
}
