using Domain.Users;

namespace Tests.Common;

public static class UsersData
{
    public static User MainUser(string password)
        => User.New(UserId.New(), "Username", "test@example.com", password, UserRole.Admin);
}
