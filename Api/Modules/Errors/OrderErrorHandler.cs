using Application.OrderItems.Exceptions;
using Application.Orders.Exceptions;
using Application.Restaurants.Exceptions;
using Application.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class OrderErrorHandler
{
    public static ObjectResult ToObjectResult(this OrderException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                OrderNotFoundException or
                    OrderRestaurantNotFoundException or
                    OrderOwnerNotFoundException => StatusCodes.Status404NotFound,
                OrderAuthorNotFoundException or
                    OrderOperationForbiddenException => StatusCodes.Status403Forbidden,
                OrderUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("User error handler does not implemented")
            }
        };
    }
}