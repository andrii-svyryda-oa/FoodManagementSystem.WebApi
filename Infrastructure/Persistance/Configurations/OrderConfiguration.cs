using Domain.Orders;
using Domain.Restaurants;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new OrderId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
        builder.Ignore(x => x.CloseOrderBillName);

        builder.Property(x => x.State)
            .IsRequired()
            .HasConversion(
                x => x.ToString(),
                x => Enum.Parse<OrderState>(x))
            .HasColumnType("varchar(50)");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .HasConstraintName("fk_orders_users_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Restaurant)
            .WithMany()
            .HasForeignKey(x => x.RestaurantId)
            .HasConstraintName("fk_orders_restaurans_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
