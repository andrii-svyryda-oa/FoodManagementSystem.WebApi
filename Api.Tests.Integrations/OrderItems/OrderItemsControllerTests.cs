using System.Net.Http.Json;
using Api.Dtos;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Restaurants;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.OrderItems;

public class OrderItemsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly Order _mainOrder;
    private readonly Restaurant _mainRestaurant;
    private readonly OrderItem _mainOrderItem;

    public OrderItemsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser("TestPassword123!");
        _mainRestaurant = RestaurantsData.MainRestaurant();
        _mainOrder = OrdersData.MainOrder(_mainRestaurant.Id, _mainUser.Id);
        _mainOrderItem = OrderItemsData.MainOrderItem(_mainOrder.Id, _mainUser.Id);
    }

    [Fact]
    public async Task ShouldGetOrderItemsByOrderId()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        // Act
        var response = await Client.GetAsync($"order-items/order/{_mainOrder.Id.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var orderItems = await response.Content.ReadFromJsonAsync<List<OrderItemDto>>();
        orderItems.Should().NotBeNull();
        orderItems!.Count.Should().Be(1);

        var orderItem = orderItems.First();
        orderItem.Id.Should().Be(_mainOrderItem.Id.Value);
        orderItem.Name.Should().Be(_mainOrderItem.Name);
        orderItem.Price.Should().Be(_mainOrderItem.Price);
        orderItem.OrderId.Should().Be(_mainOrder.Id.Value);
    }

    [Fact]
    public async Task ShouldCreateOrderItem()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());
        var request = new OrderItemDto(
            Id: null,
            Name: "New Item",
            Price: 99.99m,
            UserId: _mainUser.Id.Value,
            OrderId: _mainOrder.Id.Value,
            CreatedAt: null);

        // Act
        var response = await Client.PostAsJsonAsync("order-items", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseOrderItem = await response.Content.ReadFromJsonAsync<OrderItemDto>();
        responseOrderItem.Should().NotBeNull();
        var orderItemId = new OrderItemId(responseOrderItem!.Id!.Value);

        var dbOrderItem = await Context.OrderItems.FirstAsync(x => x.Id == orderItemId);
        dbOrderItem.Name.Should().Be(request.Name);
        dbOrderItem.Price.Should().Be(request.Price);
        dbOrderItem.OrderId.Should().Be(_mainOrder.Id);
    }

    [Fact]
    public async Task ShouldDeleteOrderItem()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());
        var orderItem = OrderItem.New(OrderItemId.New(), "Test item", 100, _mainUser.Id, _mainOrder.Id);
        await Context.OrderItems.AddAsync(orderItem);
        await SaveChangesAsync();

        // Act
        var response = await Client.DeleteAsync($"order-items/{orderItem.Id.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var orderItemExists = await Context.OrderItems.AnyAsync(x => x.Id == orderItem.Id);
        orderItemExists.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldUpdateOrderItem()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());
        var updatedName = "Updated Item Name";
        var updatedPrice = 199.99m;
        var request = new OrderItemDto(
            Id: _mainOrderItem.Id.Value,
            Name: updatedName,
            Price: updatedPrice,
            UserId: _mainUser.Id.Value,
            OrderId: _mainOrder.Id.Value,
            CreatedAt: _mainOrderItem.CreatedAt);

        // Act
        var response = await Client.PutAsJsonAsync("order-items", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var updatedOrderItem = await Context.OrderItems.FirstAsync(x => x.Id == _mainOrderItem.Id);
        updatedOrderItem.Name.Should().Be(updatedName);
        updatedOrderItem.Price.Should().Be(updatedPrice);
    }

    public async Task InitializeAsync()
    {
        await Context.Users.AddAsync(_mainUser);
        await Context.Restaurants.AddAsync(_mainRestaurant);
        await Context.Orders.AddAsync(_mainOrder);
        await Context.OrderItems.AddAsync(_mainOrderItem);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.OrderItems.RemoveRange(Context.OrderItems);
        Context.Restaurants.RemoveRange(Context.Restaurants);
        Context.Orders.RemoveRange(Context.Orders);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}
