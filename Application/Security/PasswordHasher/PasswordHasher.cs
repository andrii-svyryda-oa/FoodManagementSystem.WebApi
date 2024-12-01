using Application.Common;
using System.Security.Cryptography;

namespace Application.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128-bit salt
    private const int KeySize = 32; // 256-bit key
    private const int Iterations = 10000;
    private const string Delimiter = ":";

    public string HashPassword(string password)
    {
        var salt = GenerateSalt();
        var hash = Hash(password, salt);

        return Convert.ToBase64String(salt) + Delimiter + Convert.ToBase64String(hash);
    }

    public Result<bool, PasswordHasherException> VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split(Delimiter);
        if (parts.Length != 2)
            return new PasswordFormatIncorrectException();

        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        var computedHash = Hash(password, salt);

        if (CryptographicOperations.FixedTimeEquals(hash, computedHash))
        {
            return true;
        }
        
        return new PasswordIncorrectException();
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }

    private static byte[] Hash(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password: password,
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256
        );
        return pbkdf2.GetBytes(KeySize);
    }
}
