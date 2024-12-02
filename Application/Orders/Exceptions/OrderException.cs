using Application.OrderItems.Exceptions;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Restaurants;
using Domain.Users;

namespace Application.Orders.Exceptions;

public abstract class OrderException(OrderId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public OrderId OrderId { get; } = id;
}

public class OrderNotFoundException(OrderId id)
    : OrderException(id, $"Order with id: {id} not found");

public class OrderAlreadyClosedException(OrderId id)
    : OrderException(id, $"Order with id: {id} is already closed");

public class OrderOwnerNotFoundException(UserId ownerId)
    : OrderException(OrderId.Empty(), $"Customer with id: {ownerId} not found");

public class OrderRestaurantNotFoundException(RestaurantId restaurantId)
    : OrderException(OrderId.Empty(), $"Restuaurant with id: {restaurantId} not found");

public class OrderUnknownException(OrderId id, Exception innerException)
    : OrderException(id, $"Unknown exception for the Order with id: {id}", innerException);

public class OrderOperationForbiddenException(OrderId id)
    : OrderException(id, $"Operation for Order with id: {id} is forbidden");

public class OrderAuthorNotFoundException(UserId id)
    : OrderException(OrderId.Empty(), $"Author with id: {id} not found");
