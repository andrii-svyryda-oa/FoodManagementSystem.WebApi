using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ConfigurePersistence
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceBuild = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("Default"));
        dataSourceBuild.EnableDynamicJson();
        var dataSource = dataSourceBuild.Build();

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(
                    dataSource,
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));

        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(provider => provider.GetRequiredService<UserRepository>());
        services.AddScoped<IUserQueries>(provider => provider.GetRequiredService<UserRepository>());

        services.AddScoped<OrderRepository>();
        services.AddScoped<IOrderRepository>(provider => provider.GetRequiredService<OrderRepository>());
        services.AddScoped<IOrderQueries>(provider => provider.GetRequiredService<OrderRepository>());

        services.AddScoped<OrderItemRepository>();
        services.AddScoped<IOrderItemRepository>(provider => provider.GetRequiredService<OrderItemRepository>());
        services.AddScoped<IOrderItemQueries>(provider => provider.GetRequiredService<OrderItemRepository>());

        services.AddScoped<BalanceHistoryRepository>();
        services.AddScoped<IBalanceHistoryRepository>(provider => provider.GetRequiredService<BalanceHistoryRepository>());
        services.AddScoped<IBalanceHistoryQueries>(provider => provider.GetRequiredService<BalanceHistoryRepository>());

        services.AddScoped<RestaurantRepository>();
        services.AddScoped<IRestaurantRepository>(provider => provider.GetRequiredService<RestaurantRepository>());
        services.AddScoped<IRestaurantQueries>(provider => provider.GetRequiredService<RestaurantRepository>());
    }
}