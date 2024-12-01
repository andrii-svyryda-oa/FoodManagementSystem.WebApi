using Domain.Orders;
using Domain.OrderItems;
using Domain.Restaurants;

namespace Api.Dtos;

public record OrderDto(
    Guid? Id,
    string Name,
    Guid? OwnerId,
    DateTime? CreatedAt,
    Guid RestaurantId,
    RestaurantDto? Restaurant,
    OrderState State,
    List<OrderItemDto>? Items)
{
    public static OrderDto FromDomainModel(Order order)
        => new(
            Id: order.Id.Value,
            Name: order.Name,
            OwnerId: order.OwnerId.Value,
            CreatedAt: order.CreatedAt,
            RestaurantId: order.RestaurantId.Value,
            Restaurant: order.Restaurant == null ? null : RestaurantDto.FromDomainModel(order.Restaurant),
            State: order.State,
            Items: order.Items?.Select(OrderItemDto.FromDomainModel).ToList());
}
