using Api.Dtos;
using Application.Common.Interfaces.Queries;
using Application.Security;
using Application.Security.JwtHelper;
using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController(
        IJwtService jwtService, 
        IUserQueries userQueries,
        IPasswordHasher passwordHasher,
        IConfiguration configuration) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var user = await userQueries.GetByEmail(request.Email, cancellationToken);

            return user.Match(
                user =>
                {
                    var passwordValidated = passwordHasher.VerifyPassword(request.Password, user.Password);

                    return passwordValidated.Match<IActionResult>(
                        _ =>
                        {
                            var token = jwtService.GenerateJwtToken(user);

                            Response.Cookies.Append("JwtToken", token, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTime.UtcNow.AddMinutes(int.Parse(configuration["Jwt:ExpireMinutes"]!))
                            });

                            return Ok(new { Message = "Login successful" });
                        },
                        _ => Unauthorized("Invalid email or password."));
                },
                () => Unauthorized("Invalid email or password."));
        }

        [Authorize]
        [HttpGet("user-info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = this.GetUserIdFromClaims();

            return await userId.Match<Task<IActionResult>>(
                async userId =>
                {
                    var user = await userQueries.GetById(new UserId(userId), CancellationToken.None);

                    return user.Match<IActionResult>(
                        u => Ok(new { u.Name, u.Email }),
                        () => NotFound("User not found."));
                },
                () => Task.FromResult<IActionResult>(Unauthorized("No userId in claims.")));
        }
    }
}
