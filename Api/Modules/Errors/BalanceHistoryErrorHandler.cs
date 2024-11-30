using Application.BalanceHistories.Exceptions;
using Application.OrderItems.Exceptions;
using Application.Orders.Exceptions;
using Application.Restaurants.Exceptions;
using Application.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class BalanceHistoryErrorHandler
{
    public static ObjectResult ToObjectResult(this BalanceHistoryException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                BalanceHistoryUserNotFoundException => StatusCodes.Status404NotFound,
                BalanceHistoryUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("User error handler does not implemented")
            }
        };
    }
}