using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Password).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
        builder.Property(x => x.Balance).HasColumnType("decimal(18,2)");

        builder.Property(x => x.Role)
            .IsRequired()
            .HasConversion(
                x => x.ToString(),
                x => Enum.Parse<UserRole>(x))
            .HasColumnType("varchar(50)");
    }
}
