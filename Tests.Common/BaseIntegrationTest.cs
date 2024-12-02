using System.Data;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Domain.Orders;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestPlatform.Common;
using Optional.Collections;
using Xunit;

namespace Tests.Common;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebFactory>
{
    protected readonly ApplicationDbContext Context;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(IntegrationTestWebFactory factory)
    {
        var scope = factory.Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Configure the test client with a dynamic authentication scheme
        Client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "TestScheme", _ => { });
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    protected async Task<HttpResponseMessage> SendUnauthorizedRequest(HttpMethod method, string route, object? data = null)
    {
        var request = new HttpRequestMessage(method, route);

        if (data != null)
        {
            var jsonData = JsonSerializer.Serialize(data);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            request.Content = content;
        }

        request.Headers.Add("x-name-identifier", "");
        request.Headers.Add("x-role", "");

        return await Client.SendAsync(request);
    }

    protected void SetTestUser(string userId = "Admin", string role = "Admin")
    {
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "TestScheme");

        Client.DefaultRequestHeaders.Add("x-name-identifier", userId);
        Client.DefaultRequestHeaders.Add("x-role", role);
    }

    protected async Task<int> SaveChangesAsync()
    {
        var result = await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        return result;
    }
}

public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public static IEnumerable<Claim> Claims { get; set; } = Array.Empty<Claim>();

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>();

        if (Request.Headers.TryGetValue("x-name-identifier", out StringValues nameIdentifiers))
        {
            nameIdentifiers.FirstOrNone().MatchSome(nameIdentifier => claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier!)));
        }

        if (Request.Headers.TryGetValue("x-role", out StringValues roles))
        {
            roles.FirstOrNone().MatchSome(role => claims.Add(new Claim(ClaimTypes.Role, role!)));
        }

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
