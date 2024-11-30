using Application.Security;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public static class UserSeeder
{
    public static void Seed(EntityTypeBuilder<User> builder)
    {
        builder.HasData([
            new User(
                new UserId(new Guid("bac488f2-58e8-4546-b220-b8e9e2d0073c")), 
                "Admin", 
                "admin@gmail.com", 
                "EMko5Zy/1HcALbkaFlDXVw==:Y9zGwpaltHQJr/OWoGDNQVyyp2DuSAwWCp/dWra3wrE=", 
                UserRole.Admin, 
                0,
                new DateTime(2024, 11, 30, 18, 0, 57, 734, DateTimeKind.Utc).AddTicks(3285)
            )
        ]);
    }
}
