using Domain.Users;

namespace Tests.Common;

public static class UsersData
{
    public static User MainUser(string password)
        => User.New(new UserId(new Guid("6d2734ee-e084-4fa9-b8e7-4a7389f9f5cb")), "MainUser", "main_user@example.com", password, UserRole.Admin);

    public static User SecondaryUser(string identifier)
        => User.New(UserId.New(), "SecondaryUser" + identifier, $"test_user{identifier}@example.com", "password" + identifier, UserRole.User);
}
