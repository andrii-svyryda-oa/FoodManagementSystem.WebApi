using Api.Dtos;
using Api.Modules.Errors;
using Application.BalanceHistories.Commands;
using Application.Common.Interfaces.Queries;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController(ISender sender, IUserQueries userQueries, IConfiguration configuration) : ControllerBase
{
    [AuthorizeRoles("Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAll(cancellationToken);
        return entities.Select(UserDto.FromDomainModel).ToList();
    }

    [AuthorizeRoles("Admin")]
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(new UserId(userId), cancellationToken);
        return entity.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            () => NotFound());
    }

    [AuthorizeRoles("Admin")]
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto request, CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Role = request.Role
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [AuthorizeRoles("Admin")]
    [HttpPut]
    public async Task<ActionResult<UserDto>> Update([FromBody] UpdateUserDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateUserCommand
        {
            UserId = request.Id,
            Name = request.Name,
            Email = request.Email
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            user => UserDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }

    [AuthorizeRoles("Admin")]
    [HttpPut("{userId:guid}/update-balance")]
    public async Task<ActionResult<BalanceHistoryDto>> UpdateBalance([FromRoute] Guid userId, [FromBody] AdjustUserBalanceDto request, CancellationToken cancellationToken)
    {
        var input = new ChangeUserBalanceCommand
        {
            UserId = userId,
            Details = request.Details,
            Difference = request.Difference
        };

        var result = await sender.Send(input, cancellationToken);

        return await result.MatchAsync(
            async _ =>
            {
                var input = new CreateBalanceHistoryCommand
                {
                    UserId = userId,
                    Details = request.Details,
                    Difference = request.Difference
                };

                var result = await sender.Send(input, cancellationToken);

                return result.Match<ActionResult<BalanceHistoryDto>>(
                    balanceHistory => BalanceHistoryDto.FromDomainModel(balanceHistory),
                    e => e.ToObjectResult());
            },
            e => e.ToObjectResult());
    }

    [AuthorizeRoles("Admin")]
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var input = new DeleteUserCommand
        {
            UserId = userId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult());
    }
}
