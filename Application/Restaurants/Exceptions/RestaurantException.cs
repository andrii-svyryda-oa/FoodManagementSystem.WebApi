using Domain.Restaurants;
using System;

namespace Application.Restaurants.Exceptions;

public abstract class RestaurantException(RestaurantId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public RestaurantId RestaurantId { get; } = id;
}

public class RestaurantNotFoundException(RestaurantId id)
    : RestaurantException(id, $"Restaurant with id {id} not found.");

public class RestaurantAlreadyExistsException(RestaurantId id)
    : RestaurantException(id, $"Restaurant with id {id} already exists.");

public class RestaurantUnknownException(RestaurantId id, Exception innerException)
    : RestaurantException(id, $"An unknown error occurred for restaurant with id {id}.", innerException);
