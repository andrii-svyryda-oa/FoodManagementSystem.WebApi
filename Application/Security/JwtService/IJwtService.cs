using Domain.Users;
using Optional;
using System.Security.Claims;

namespace Application.Security.JwtHelper
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
    }
}