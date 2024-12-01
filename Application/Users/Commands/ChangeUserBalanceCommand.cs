using Application.Common;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record ChangeUserBalanceCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required string Details { get; init; }
    public required decimal Difference { get; init; }
}
