namespace Domain.User;

public class User
{
    public UserId Id { get; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string Role { get; private set; } // Admin | User
    public DateTime CreatedAt { get; private set; }
    public decimal Balance { get; private set; }

    private User(UserId id, string name, string email, string password, string role, decimal balance, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Role = role;
        Balance = balance;
        CreatedAt = createdAt;
    }

    public static User New(UserId id, string name, string email, string password, string role, decimal balance)
        => new(id, name, email, password, role, balance, DateTime.UtcNow);

    public void UpdateDetails(string name, string email, string role)
    {
        Name = name;
        Email = email;
        Role = role;
    }

    public void UpdateBalance(decimal difference)
    {
        Balance += difference;
    }
}
