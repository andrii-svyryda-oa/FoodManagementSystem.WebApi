using Application.Common;
using Optional;

namespace Application.Security;

public interface IPasswordHasher
{
    string HashPassword(string password);
    Result<bool, PasswordHasherException> VerifyPassword(string password, string hashedPassword);
}
