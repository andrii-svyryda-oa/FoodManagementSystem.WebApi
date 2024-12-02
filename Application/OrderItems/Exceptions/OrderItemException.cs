using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;

namespace Application.OrderItems.Exceptions;

public abstract class OrderItemException(OrderItemId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public OrderItemId OrderItemId { get; } = id;
}

public class OrderItemNotFoundException(OrderItemId id)
    : OrderItemException(id, $"OrderItem with id: {id} not found");

public class OrderItemOrderAlreadyClosedException(OrderItemId id)
    : OrderItemException(id, $"OrderItem with id: {id} operation now allowed cause order is already closed");

public class OrderItemOperationForbiddenException(OrderItemId id)
    : OrderItemException(id, $"Operation for OrderItem with id: {id} is forbidden");

public class OrderItemAuthorNotFoundException(UserId id)
    : OrderItemException(OrderItemId.Empty(), $"Author with id: {id} not found");

public class OrderItemOrderNotFoundException(OrderId id)
    : OrderItemException(OrderItemId.Empty(), $"Order with id: {id} not found");

public class OrderItemUserNotFoundException(UserId id)
    : OrderItemException(OrderItemId.Empty(), $"User with id: {id} not found");

public class OrderItemUnknownException(OrderItemId id, Exception innerException)
    : OrderItemException(id, $"Unknown exception for the OrderItem with id: {id}", innerException);
