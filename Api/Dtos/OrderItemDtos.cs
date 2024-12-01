using Domain.OrderItems;

namespace Api.Dtos;

public record OrderItemDto(
    Guid? Id,
    string Name,
    decimal Price,
    Guid UserId,
    Guid? OrderId,
    DateTime? CreatedAt)
{
    public static OrderItemDto FromDomainModel(OrderItem orderItem)
        => new(
            Id: orderItem.Id.Value,
            Name: orderItem.Name,
            Price: orderItem.Price,
            UserId: orderItem.UserId.Value,
            OrderId: orderItem.OrderId.Value,
            CreatedAt: orderItem.CreatedAt);
}
