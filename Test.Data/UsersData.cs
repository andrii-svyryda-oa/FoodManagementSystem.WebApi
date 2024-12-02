using Domain.Users;

namespace Tests.Common;

public static class UsersData
{
    public static User MainUser(string password)
        => User.New(UserId.New(), "MainUser", "main_user@example.com", password, UserRole.Admin);

    public static User SecondaryUser(string identifier)
        => User.New(UserId.New(), "SecondaryUser" + identifier, $"test_user{identifier}@example.com", "password" + identifier, UserRole.User);
}
