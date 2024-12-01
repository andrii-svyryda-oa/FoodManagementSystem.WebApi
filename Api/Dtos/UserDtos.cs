using Domain.Users;

namespace Api.Dtos;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    UserRole Role,
    DateTime CreatedAt,
    decimal Balance)
{
    public static UserDto FromDomainModel(User user)
        => new(
            Id: user.Id.Value,
            Name: user.Name,
            Email: user.Email,
            Role: user.Role,
            CreatedAt: user.CreatedAt,
            Balance: user.Balance);
}

public record CreateUserDto(
    string Name,
    string Email,
    string Password,
    UserRole Role);

public record LoginRequestDto(
    string Email,
    string Password);

public record UpdateUserDto(
    Guid Id,
    string Name,
    string Email);

public record AdjustUserBalanceDto(
    string Details,
    decimal Difference);
