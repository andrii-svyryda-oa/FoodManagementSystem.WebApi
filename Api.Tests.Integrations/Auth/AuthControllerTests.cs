using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Application.Security;
using Domain.BalanceHistories;
using Domain.OrderItems;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.Auth;

public class AuthControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly IPasswordHasher _passwordHasher;

    public AuthControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // Hash the password when creating mainUser
        _mainUser = UsersData.MainUser(_passwordHasher.HashPassword("Password123!"));
    }

    [Fact]
    public async Task ShouldLoginSuccessfully()
    {
        // Arrange
        var request = new LoginRequestDto(_mainUser.Email, "Password123!");

        // Act
        var response = await Client.PostAsJsonAsync("auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("Set-Cookie");
    }

    [Fact]
    public async Task ShouldFailLoginWithInvalidPassword()
    {
        // Arrange
        var request = new LoginRequestDto(_mainUser.Email, "WrongPassword");

        // Act
        var response = await Client.PostAsJsonAsync("auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task ShouldFailLoginWithInvalidEmail()
    {
        // Arrange
        var request = new LoginRequestDto("nonexistent@example.com", "Password123!");

        // Act
        var response = await Client.PostAsJsonAsync("auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task ShouldRetrieveUserInfoSuccessfully()
    {
        // Act
        var response = await Client.GetAsync("auth/user-info");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        responseContent.Should().ContainKey("name");
        responseContent.Should().ContainKey("email");
        responseContent!["name"].Should().Be(_mainUser.Name);
        responseContent!["email"].Should().Be(_mainUser.Email);
    }

    [Fact]
    public async Task ShouldReturnUnauthorizedForUserInfoWithoutAuthentication()
    {
        // Act
        var response = await SendUnauthorizedRequest(HttpMethod.Get, "auth/user-info");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public async Task InitializeAsync()
    {
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        await Context.Users.AddAsync(_mainUser);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}
