using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserQueries
{
    Task<(IReadOnlyList<User>, int)> GetAll(int skip, int take, string searchText, CancellationToken cancellationToken);
    Task<Option<User>> GetById(UserId id, CancellationToken cancellationToken);
    Task<Option<User>> GetByEmail(string email, CancellationToken cancellationToken);
}
