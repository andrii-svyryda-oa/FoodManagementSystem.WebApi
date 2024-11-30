using Domain.Restaurants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new RestaurantId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Description).HasColumnType("varchar(500)");
    }
}
