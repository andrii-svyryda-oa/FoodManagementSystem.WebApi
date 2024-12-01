using Api.Dtos;
using Application.Common.Interfaces.Queries;
using Application.Orders.Commands;
using Domain.Orders;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Api.Controllers;

[Route("balance-history")]
[ApiController]
public class BalanceHistoryController(IBalanceHistoryQueries balanceHistoryQueries) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BalanceHistoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var userId = this.GetUserIdFromClaims();

        return await userId.Match<Task<ActionResult<IReadOnlyList<BalanceHistoryDto>>>>(
            async userId =>
            {
                var entities = await balanceHistoryQueries.GetByUserId(new UserId(userId), cancellationToken);

                return entities.Select(BalanceHistoryDto.FromDomainModel).ToList();
            },
            () => Task.FromResult<ActionResult<IReadOnlyList<BalanceHistoryDto>>>(Unauthorized()));
    }
}
