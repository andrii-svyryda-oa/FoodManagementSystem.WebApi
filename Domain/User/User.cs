namespace Domain.Users;

public enum UserRole
{
    Admin,
    User
}

public class User
{
    public UserId Id { get; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal Balance { get; private set; }

    public User(UserId id, string name, string email, string password, UserRole role, decimal balance, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Role = role;
        Balance = balance;
        CreatedAt = createdAt;
    }

    public static User New(UserId id, string name, string email, string password, UserRole role)
        => new(id, name, email, password, role, 0, DateTime.UtcNow);

    public void UpdateDetails(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public void UpdateBalance(decimal difference)
    {
        Balance += difference;
    }
}
