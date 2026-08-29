using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private const string Email = "dev@martech.com";
    private const string Password = "Senha@123";

    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        if (request.Email != Email ||
            request.Password != Password)
        {
            return Unauthorized();
        }

        var jwtSection =
            _configuration.GetRequiredSection("Jwt");

        var key = jwtSection["Key"]
            ?? throw new InvalidOperationException(
                "JWT key is not configured.");

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                request.Email),

            new Claim(
                JwtRegisteredClaimNames.Email,
                request.Email)
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var expirationMinutes =
            jwtSection.GetValue<int>("ExpirationMinutes");

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                expirationMinutes),
            signingCredentials: credentials);

        var tokenValue =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return Ok(new
        {
            accessToken = tokenValue
        });
    }

    public sealed record LoginRequest(
        string Email,
        string Password);
}