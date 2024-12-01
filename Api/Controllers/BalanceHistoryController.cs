using Api.Dtos;
using Application.Common.Interfaces.Queries;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("balance-history")]
[ApiController]
public class BalanceHistoryController(IBalanceHistoryQueries balanceHistoryQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BalanceHistoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await balanceHistoryQueries.GetByUserId(UserId.New(), cancellationToken);

        return entities.Select(BalanceHistoryDto.FromDomainModel).ToList();
    }
}
