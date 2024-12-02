using Api.Dtos;
using Api.Modules.Errors;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Orders.Commands;
using Application.Orders.Exceptions;
using Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("orders")]
[ApiController]
public class OrdersController(ISender sender, IOrderQueries orderQueries) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await orderQueries.GetAll(cancellationToken);

        return entities.Select(OrderDto.FromDomainModel).ToList();
    }

    [Authorize]
    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> Get([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var entity = await orderQueries.GetById(new OrderId(orderId), cancellationToken);

        return entity.Match<ActionResult<OrderDto>>(
            o => OrderDto.FromDomainModel(o),
            () => NotFound());
    }

    [Authorize]
    [HttpPost("{orderId:guid}/close")]
    public async Task<ActionResult<OrderDto>> CloseOrder([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var userId = this.GetUserIdFromClaims();

        return await userId.Match<Task<ActionResult<OrderDto>>>(
            async userId =>
            {
                var input = new CloseOrderCommand
                {
                    OrderId = orderId,
                    AuthorId = userId
                };

                var result = await sender.Send<Result<Order, OrderException>>(input, cancellationToken);

                return result.Match<ActionResult<OrderDto>>(
                    o => OrderDto.FromDomainModel(o),
                    e => e.ToObjectResult());
            },
            () => Task.FromResult<ActionResult<OrderDto>>(base.Unauthorized()));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderDto request, CancellationToken cancellationToken)
    {
        var userId = this.GetUserIdFromClaims();

        return await userId.Match<Task<ActionResult<OrderDto>>>(
            async userId =>
            {
                var input = new CreateOrderCommand
                {
                    Name = request.Name,
                    OwnerId = userId,
                    RestaurantId = request.RestaurantId
                };

                var result = await sender.Send(input, cancellationToken);

                return result.Match<ActionResult<OrderDto>>(
                    o => OrderDto.FromDomainModel(o),
                    e => e.ToObjectResult());
            },
            () => Task.FromResult<ActionResult<OrderDto>>(Unauthorized()));
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<OrderDto>> Update([FromBody] OrderDto request, CancellationToken cancellationToken)
    {
        var userId = this.GetUserIdFromClaims();

        return await userId.Match<Task<ActionResult<OrderDto>>>(
            async userId =>
            {
                var input = new UpdateOrderCommand
                {
                    OrderId = request.Id!.Value,
                    Name = request.Name,
                    State = request.State,
                    AuthorId = userId,
                };

                var result = await sender.Send(input, cancellationToken);

                return result.Match<ActionResult<OrderDto>>(
                    o => OrderDto.FromDomainModel(o),
                    e => e.ToObjectResult());
            },
            () => Task.FromResult<ActionResult<OrderDto>>(Unauthorized()));
    }

    [Authorize]
    [HttpDelete("{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> Delete([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var userId = this.GetUserIdFromClaims();

        return await userId.Match<Task<ActionResult<OrderDto>>>(
            async userId =>
            {
                var input = new DeleteOrderCommand
                {
                    OrderId = orderId,
                    AuthorId = userId
                };

                var result = await sender.Send(input, cancellationToken);

                return result.Match<ActionResult<OrderDto>>(
                    o => OrderDto.FromDomainModel(o),
                    e => e.ToObjectResult());
            },
            () => Task.FromResult<ActionResult<OrderDto>>(Unauthorized()));
    }
}
