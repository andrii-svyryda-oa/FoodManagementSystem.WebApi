using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Restaurants.Commands;
using Domain.Restaurants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("restaurants")]
[ApiController]
public class RestaurantsController(ISender sender, IRestaurantQueries restaurantQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RestaurantDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await restaurantQueries.GetAll(cancellationToken);

        return entities.Select(RestaurantDto.FromDomainModel).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<RestaurantDto>> Create([FromBody] RestaurantDto request, CancellationToken cancellationToken)
    {
        var input = new CreateRestaurantCommand
        {
            Name = request.Name,
            Description = request.Description
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RestaurantDto>>(
            r => RestaurantDto.FromDomainModel(r),
            e => e.ToObjectResult());
    }

    [HttpPut]
    public async Task<ActionResult<RestaurantDto>> Update([FromBody] RestaurantDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateRestaurantCommand
        {
            RestaurantId = request.Id!.Value,
            Name = request.Name,
            Description = request.Description
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RestaurantDto>>(
            r => RestaurantDto.FromDomainModel(r),
            e => e.ToObjectResult());
    }

    [HttpDelete("{restaurantId:guid}")]
    public async Task<ActionResult<RestaurantDto>> Delete([FromRoute] Guid restaurantId, CancellationToken cancellationToken)
    {
        var input = new DeleteRestaurantCommand
        {
            RestaurantId = restaurantId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RestaurantDto>>(
            r => RestaurantDto.FromDomainModel(r),
            e => e.ToObjectResult());
    }
}
