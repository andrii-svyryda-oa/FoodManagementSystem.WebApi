using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> Add(User user, CancellationToken cancellationToken);
    Task<User> Update(User user, CancellationToken cancellationToken);
    Task<List<User>> UpdateMany(List<User> users, CancellationToken cancellationToken);
    Task<User> Delete(User user, CancellationToken cancellationToken);
    Task<Option<User>> GetById(UserId id, CancellationToken cancellationToken);
    Task<List<User>> GetByIds(List<UserId> ids, CancellationToken cancellationToken);
    Task<Option<User>> GetByEmail(string email, CancellationToken cancellationToken);
}
