using System.Net.Http.Json;
using Api.Dtos;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;

    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser(password: "TestPassword123!");
    }

    [Fact]
    public async Task ShouldGetAllUsers()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        // Act
        var response = await Client.GetAsync("users");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        users.Should().NotBeNull();
        users!.Count.Should().BeGreaterThan(0);

        var user = users.First(x => x.Id == _mainUser.Id.Value);
        user.Name.Should().Be(_mainUser.Name);
        user.Email.Should().Be(_mainUser.Email);
    }

    [Fact]
    public async Task ShouldGetUserById()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        var userId = _mainUser.Id.Value;

        // Act
        var response = await Client.GetAsync($"users/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(_mainUser.Id.Value);
        user.Name.Should().Be(_mainUser.Name);
        user.Email.Should().Be(_mainUser.Email);
    }

    [Fact]
    public async Task ShouldCreateUser()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        var createdName = "Created User Name";
        var createdEmail = "created@example.com";
        var request = new CreateUserDto(createdName, createdEmail, "Password123", UserRole.User);

        // Act
        var response = await Client.PostAsJsonAsync("users", request);

        var responseUser = await response.ToResponseModel<UserDto>();
        var userId = new UserId(responseUser.Id);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var updatedUser = await Context.Users.FirstAsync(x => x.Id == userId);
        updatedUser.Name.Should().Be(createdName);
        updatedUser.Email.Should().Be(createdEmail);
    }

    [Fact]
    public async Task ShouldUpdateUser()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        var updatedName = "Updated User Name";
        var updatedEmail = "updated@example.com";
        var request = new UpdateUserDto(_mainUser.Id.Value, updatedName, updatedEmail);

        // Act
        var response = await Client.PutAsJsonAsync("users", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var updatedUser = await Context.Users.FirstAsync(x => x.Id == _mainUser.Id);
        updatedUser.Name.Should().Be(updatedName);
        updatedUser.Email.Should().Be(updatedEmail);
    }

    [Fact]
    public async Task ShouldUpdateUserBalanceAndCreateBalanceHistory()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        var difference = 1000m;
        var details = "Test Balance Update";
        var request = new AdjustUserBalanceDto(details, difference);

        // Act
        var response = await Client.PutAsJsonAsync($"users/{_mainUser.Id.Value}/update-balance", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify balance was updated
        var updatedUser = await Context.Users.FirstAsync(x => x.Id == _mainUser.Id);
        updatedUser.Balance.Should().Be(difference);

        // Verify balance history was created
        var balanceHistory = await Context.BalanceHistory
            .FirstOrDefaultAsync(x => x.UserId == _mainUser.Id && x.Details == details);
        balanceHistory.Should().NotBeNull();
        balanceHistory!.Difference.Should().Be(difference);
    }

    [Fact]
    public async Task ShouldDeleteUser()
    {
        // Arrange
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        var testUser = User.New(UserId.New(), "delete test user", "delete@example.com", "passwd", UserRole.Admin);
        await Context.Users.AddAsync(testUser);
        await SaveChangesAsync();
        var userId = testUser.Id.Value;

        // Act
        var response = await Client.DeleteAsync($"users/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var userExists = await Context.Users.AnyAsync(x => x.Id == testUser.Id);
        userExists.Should().BeFalse();
    }

    public async Task InitializeAsync()
    {
        await Context.Users.AddAsync(_mainUser);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.BalanceHistory.RemoveRange(Context.BalanceHistory);
        await SaveChangesAsync();
    }
}
