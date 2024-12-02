using System.Net.Http.Json;
using Api.Dtos;
using Domain.BalanceHistories;
using Domain.OrderItems;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.BalanceHistories;

public class BalanceHistoryControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly BalanceHistory _mainBalanceHistory;

    public BalanceHistoryControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser("TestPassword123!");
        _mainBalanceHistory = BalanceHistoryData.MainBalanceHistory(_mainUser.Id);
    }

    [Fact]
    public async Task ShouldGetAllBalanceHistoryForUser()
    {
        // Act
        var response = await Client.GetAsync("balance-history");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var balanceHistories = await response.Content.ReadFromJsonAsync<List<BalanceHistoryDto>>();
        balanceHistories.Should().NotBeNull();
        balanceHistories!.Count.Should().Be(1);

        var balanceHistory = balanceHistories.First();
        balanceHistory.Id.Should().Be(_mainBalanceHistory.Id.Value);
        balanceHistory.Details.Should().Be(_mainBalanceHistory.Details);
        balanceHistory.Difference.Should().Be(_mainBalanceHistory.Difference);
    }

    public async Task InitializeAsync()
    {
        SetTestUser(_mainUser.Id.ToString(), _mainUser.Role.ToString());

        await Context.Users.AddAsync(_mainUser);
        await Context.BalanceHistory.AddAsync(_mainBalanceHistory);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.BalanceHistory.RemoveRange(Context.BalanceHistory);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}
