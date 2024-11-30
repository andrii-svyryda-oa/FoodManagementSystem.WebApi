using Domain.BalanceHistories;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BalanceHistoryConfiguration : IEntityTypeConfiguration<BalanceHistory>
{
    public void Configure(EntityTypeBuilder<BalanceHistory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new BalanceHistoryId(x));

        builder.Property(x => x.Details).IsRequired().HasColumnType("varchar(500)");
        builder.Property(x => x.Difference).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("timezone('utc', now())");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("fk_balance_histories_users_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
