using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using Bupa.Books.PublicApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Bupa.Books.PublicApi.Controllers;

/// <summary>
/// Handles authentication for the Bupa Books Public API.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// Issues a short-lived JWT bearer token in exchange for valid credentials.
    /// </summary>
    /// <param name="request">The login credentials (username and password).</param>
    /// <returns>A JWT bearer token valid for 1 hour.</returns>
    /// <response code="200">Token successfully issued.</response>
    /// <response code="401">Invalid username or password.</response>
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetToken([FromBody] LoginRequest request)
    {
        var expectedUsername = configuration["MockAuth:Username"];
        var expectedPassword = configuration["MockAuth:Password"];

        if (request.Username != expectedUsername || request.Password != expectedPassword)
            return Unauthorized(new { error = "Invalid credentials" });

        return Ok(new TokenResponse(GenerateJwtToken(), "Bearer", 3600));
    }

    private string GenerateJwtToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "demo-user"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "admin")
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
