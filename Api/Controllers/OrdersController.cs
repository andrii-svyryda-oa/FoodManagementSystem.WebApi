using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Orders.Commands;
using Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("orders")]
[ApiController]
public class OrdersController(ISender sender, IOrderQueries orderQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await orderQueries.GetAll(cancellationToken);

        return entities.Select(OrderDto.FromDomainModel).ToList();
    }

    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> Get([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var entity = await orderQueries.GetById(new OrderId(orderId), cancellationToken);
        
        return entity.Match<ActionResult<OrderDto>>(
            o => OrderDto.FromDomainModel(o),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderDto request, CancellationToken cancellationToken)
    {
        var input = new CreateOrderCommand
        {
            Name = request.Name,
            OwnerId = request.OwnerId,
            RestaurantId = request.RestaurantId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<OrderDto>>(
            o => OrderDto.FromDomainModel(o),
            e => e.ToObjectResult());
    }

    [HttpPut]
    public async Task<ActionResult<OrderDto>> Update([FromBody] OrderDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateOrderCommand
        {
            OrderId = request.Id!.Value,
            Name = request.Name,
            State = request.State
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<OrderDto>>(
            o => OrderDto.FromDomainModel(o),
            e => e.ToObjectResult());
    }

    [HttpDelete("{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> Delete([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var input = new DeleteOrderCommand
        {
            OrderId = orderId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<OrderDto>>(
            o => OrderDto.FromDomainModel(o),
            e => e.ToObjectResult());
    }
}
