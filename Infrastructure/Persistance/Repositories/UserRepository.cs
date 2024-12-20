﻿using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository, IUserQueries
{
    public async Task<Option<User>> GetById(UserId id, CancellationToken cancellationToken)
    {
        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<User>() : Option.Some(entity);
    }


    public async Task<List<User>> GetByIds(List<UserId> ids, CancellationToken cancellationToken)
    {
        var entities = await context.Users
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<(IReadOnlyList<User>, int)> GetAll(int skip, int take, string searchText, CancellationToken cancellationToken)
    {
        var query = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var pattern = $"%{searchText.ToLower()}%";

            query = query.Where(
                x => EF.Functions.Like(x.Name.ToLower(), pattern) &&
                EF.Functions.Like(x.Email.ToLower(), pattern));
        }

        var count = await query.CountAsync(cancellationToken);
        var result = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);

        return (result, count);
    }

    public async Task<Option<User>> GetByEmail(string email, CancellationToken cancellationToken)
    {
        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        return entity == null ? Option.None<User>() : Option.Some(entity);
    }

    public async Task<User> Add(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<List<User>> UpdateMany(List<User> users, CancellationToken cancellationToken)
    {
        context.Users.UpdateRange(users);
        await context.SaveChangesAsync(cancellationToken);
        return users;
    }

    public async Task<User> Update(User user, CancellationToken cancellationToken)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> Delete(User user, CancellationToken cancellationToken)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }
}
