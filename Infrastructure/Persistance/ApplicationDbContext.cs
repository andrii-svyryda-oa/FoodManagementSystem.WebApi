using System.Reflection;
using Application.Security;
using Domain.BalanceHistories;
using Domain.OrderItems;
using Domain.Orders;
using Domain.Restaurants;
using Domain.Users;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IPasswordHasher passwordHasher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPasswordHasher passwordHasher)
        : base(options)
    {
        this.passwordHasher = passwordHasher;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<BalanceHistory> BalanceHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        UserSeeder.Seed(builder.Entity<User>(), passwordHasher);

        base.OnModelCreating(builder);
    }
}