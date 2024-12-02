using System.Net.Http.Json;
using Api.Dtos;
using Domain.Restaurants;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.Restaurants;

public class RestaurantsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly Restaurant _mainRestaurant;

    public RestaurantsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser("TestPassword123!");
        _mainRestaurant = RestaurantsData.MainRestaurant();
    }

    [Fact]
    public async Task ShouldGetAllRestaurants()
    {
        // Act
        var response = await Client.GetAsync("restaurants");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var restaurants = await response.Content.ReadFromJsonAsync<List<RestaurantDto>>();
        restaurants.Should().NotBeNull();
        restaurants!.Count.Should().Be(1);

        var restaurant = restaurants.First();
        restaurant.Id.Should().Be(_mainRestaurant.Id.Value);
        restaurant.Name.Should().Be(_mainRestaurant.Name);
        restaurant.Description.Should().Be(_mainRestaurant.Description);
    }

    [Fact]
    public async Task ShouldCreateRestaurant()
    {
        // Arrange
        var request = new RestaurantDto(
            Id: null,
            Name: "New Restaurant",
            Description: "A brand new restaurant");

        // Act
        var response = await Client.PostAsJsonAsync("restaurants", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseRestaurant = await response.Content.ReadFromJsonAsync<RestaurantDto>();
        responseRestaurant.Should().NotBeNull();
        var restaurantId = new RestaurantId(responseRestaurant!.Id!.Value);

        var dbRestaurant = await Context.Restaurants.FirstAsync(x => x.Id == restaurantId);
        dbRestaurant.Name.Should().Be(request.Name);
        dbRestaurant.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task ShouldUpdateRestaurant()
    {
        // Arrange
        var updatedName = "Updated Restaurant Name";
        var updatedDescription = "Updated Description";
        var request = new RestaurantDto(
            Id: _mainRestaurant.Id.Value,
            Name: updatedName,
            Description: updatedDescription);

        // Act
        var response = await Client.PutAsJsonAsync("restaurants", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var updatedRestaurant = await Context.Restaurants.FirstAsync(x => x.Id == _mainRestaurant.Id);
        updatedRestaurant.Name.Should().Be(updatedName);
        updatedRestaurant.Description.Should().Be(updatedDescription);
    }

    [Fact]
    public async Task ShouldDeleteRestaurant()
    {
        // Arrange
        var testRestaurant = Restaurant.New(RestaurantId.New(), "Delete restaurant test", "delete test");
        await Context.Restaurants.AddAsync(testRestaurant);
        await SaveChangesAsync();
        var restaurantId = testRestaurant.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"restaurants/{restaurantId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var restaurantExists = await Context.Restaurants.AnyAsync(x => x.Id == testRestaurant.Id);
        restaurantExists.Should().BeFalse();
    }

    public async Task InitializeAsync()
    {
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        await Context.Users.AddAsync(_mainUser);
        await Context.Restaurants.AddAsync(_mainRestaurant);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Restaurants.RemoveRange(Context.Restaurants);
        await SaveChangesAsync();
    }
}
