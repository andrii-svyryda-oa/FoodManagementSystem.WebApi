using Api.Dtos;
using Api.Modules.Errors;
using Application.BalanceHistories.Commands;
using Application.Common.Interfaces.Queries;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController(ISender sender, IUserQueries userQueries, IConfiguration configuration) : ControllerBase
{
    //[AuthorizeRoles("Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAll(cancellationToken);
        return entities.Select(UserDto.FromDomainModel).ToList();
    }

    /*[Authorize]
    [HttpGet("get-me)]
    public async Task<ActionResult<UserDto>> Get(CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(new UserId(userId), cancellationToken);
        return entity.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            () => NotFound());
    }*/

    //[Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(new UserId(userId), cancellationToken);
        return entity.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            () => NotFound());
    }

    /*[HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserDto request, CancellationToken cancellationToken)
    {
        var input = new RegisterUserCommand
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
    }*/

    /*[HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await userQueries.GetByEmail(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = JwtHelper.GenerateJwtToken(user, _configuration);

        // Set JWT token in a cookie
        Response.Cookies.Append("JwtToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"]))
        });

        return Ok(new { Message = "Login successful" });
    }*/

    //[Authorize]
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

    //[AuthorizeRoles("Admin")]
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
