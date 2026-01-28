using Microsoft.AspNetCore.Mvc;
using SecureFileDelivery.Application.Interfaces;

namespace SecureFileDelivery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtTokenService jwtTokenService,
        ILogger<AuthController> logger)
    {
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    // Generate a JWT token for authentication
    // In production, this would validate credentials against a user database
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        // In production: validate username/password against database
        // For demo: accept any non-empty credentials
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return Unauthorized(new { error = "Invalid credentials" });
        }

        // Generate token
        var token = _jwtTokenService.GenerateToken(
            userId: Guid.NewGuid().ToString(),
            userName: request.Username,
            roles: new[] { "User" }
        );

        _logger.LogInformation("JWT token generated for user {Username}", request.Username);

        return Ok(new TokenResponse
        {
            Token = token,
            ExpiresIn = 3600, // 1 hour in seconds
            TokenType = "Bearer"
        });
    }
}

public class TokenRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; } = string.Empty;
}