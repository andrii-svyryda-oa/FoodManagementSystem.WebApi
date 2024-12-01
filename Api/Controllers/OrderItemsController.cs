using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.OrderItems.Commands;
using Domain.OrderItems;
using Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("order-items")]
[ApiController]
public class OrderItemsController(ISender sender, IOrderItemQueries orderItemQueries) : ControllerBase
{
    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<IReadOnlyList<OrderItemDto>>> GetByOrderId([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var entities = await orderItemQueries.GetByOrderId(new OrderId(orderId), cancellationToken);

        return entities.Select(OrderItemDto.FromDomainModel).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<OrderItemDto>> Create([FromBody] OrderItemDto request, CancellationToken cancellationToken)
    {
        var input = new CreateOrderItemCommand
        {
            Name = request.Name,
            Price = request.Price,
            UserId = request.UserId,
            OrderId = request.OrderId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<OrderItemDto>>(
            oi => OrderItemDto.FromDomainModel(oi),
            e => e.ToObjectResult());
    }

    [HttpPut]
    public async Task<ActionResult<OrderItemDto>> Update([FromBody] OrderItemDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateOrderItemCommand
        {
            OrderItemId = request.Id!.Value,
            Name = request.Name,
            Price = request.Price
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<OrderItemDto>>(
            oi => OrderItemDto.FromDomainModel(oi),
            e => e.ToObjectResult());
    }

    [HttpDelete("{orderItemId:guid}")]
    public async Task<ActionResult<OrderItemDto>> Delete([FromRoute] Guid orderItemId, CancellationToken cancellationToken)
    {
        var input = new DeleteOrderItemCommand
        {
            OrderItemId = orderItemId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<OrderItemDto>>(
            oi => OrderItemDto.FromDomainModel(oi),
            e => e.ToObjectResult());
    }
}
