using Application.Restaurants.Exceptions;
using Application.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class RestaurantErrorHandler
{
    public static ObjectResult ToObjectResult(this RestaurantException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                RestaurantNotFoundException => StatusCodes.Status404NotFound,
                RestaurantAlreadyExistsException => StatusCodes.Status409Conflict,
                RestaurantUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("User error handler does not implemented")
            }
        };
    }
}