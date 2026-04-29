namespace Bupa.Books.PublicApi.Models;

/// <summary>Credentials submitted to the token endpoint to obtain a JWT access token.</summary>
/// <param name="Username">The account username.</param>
/// <param name="Password">The account password.</param>
public record LoginRequest(string Username, string Password);

/// <summary>JWT bearer token returned after successful authentication.</summary>
/// <param name="AccessToken">The signed JWT value to supply in the <c>Authorization: Bearer</c> header.</param>
/// <param name="TokenType">Always <c>Bearer</c>.</param>
/// <param name="ExpiresIn">Token lifetime in seconds from the time of issue.</param>
public record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);
