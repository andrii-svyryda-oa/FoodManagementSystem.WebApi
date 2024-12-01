using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.OrderItems.Commands;
using Application.Orders.Commands;
using Domain.OrderItems;
using Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("order-items")]
[ApiController]
public class OrderItemsController(ISender sender, IOrderItemQueries orderItemQueries) : ControllerBase
{
    [Authorize]
    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<IReadOnlyList<OrderItemDto>>> GetByOrderId([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var entities = await orderItemQueries.GetByOrderId(new OrderId(orderId), cancellationToken);

        return entities.Select(OrderItemDto.FromDomainModel).ToList();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<OrderItemDto>> Create([FromBody] OrderItemDto request, CancellationToken cancellationToken)
    {
        var userId = this.GetUserIdFromClaims();

        return await userId.Match<Task<ActionResult<OrderItemDto>>>(
            async userId =>
            {
                var input = new CreateOrderItemCommand
                {
                    Name = request.Name,
                    Price = request.Price,
                    UserId = request.UserId,
                    OrderId = request.OrderId ?? userId
                };

                var result = await sender.Send(input, cancellationToken);

                return result.Match<ActionResult<OrderItemDto>>(
                    oi => OrderItemDto.FromDomainModel(oi),
                    e => e.ToObjectResult());
            },
            () => Task.FromResult<ActionResult<OrderItemDto>>(Unauthorized()));
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<OrderItemDto>> Update([FromBody] OrderItemDto request, CancellationToken cancellationToken)
    {
        var userId = this.GetUserIdFromClaims();

        return await userId.Match<Task<ActionResult<OrderItemDto>>>(
            async userId =>
            {
                var input = new UpdateOrderItemCommand
                {
                    OrderItemId = request.Id!.Value,
                    Name = request.Name,
                    Price = request.Price,
                    UserId = userId
                };

                var result = await sender.Send(input, cancellationToken);

                return result.Match<ActionResult<OrderItemDto>>(
                    oi => OrderItemDto.FromDomainModel(oi),
                    e => e.ToObjectResult());
            },
            () => Task.FromResult<ActionResult<OrderItemDto>>(Unauthorized()));
    }

    [Authorize]
    [HttpDelete("{orderItemId:guid}")]
    public async Task<ActionResult<OrderItemDto>> Delete([FromRoute] Guid orderItemId, CancellationToken cancellationToken)
    {
        var userId = this.GetUserIdFromClaims();

        return await userId.Match<Task<ActionResult<OrderItemDto>>>(
            async userId =>
            {
                var input = new DeleteOrderItemCommand
                {
                    OrderItemId = orderItemId,
                    UserId = userId
                };

                var result = await sender.Send(input, cancellationToken);

                return result.Match<ActionResult<OrderItemDto>>(
                    oi => OrderItemDto.FromDomainModel(oi),
                    e => e.ToObjectResult());
            },
            () => Task.FromResult<ActionResult<OrderItemDto>>(Unauthorized()));
    }
}
