namespace SecureFileDelivery.Application.Interfaces;

// Service for generating and validating JWT tokens
public interface IJwtTokenService
{
    // Generates a JWT token for a user
    string GenerateToken(string userId, string userName, IEnumerable<string>? roles = null);

    // Validates a JWT token and returns the user ID if valid
    string? ValidateToken(string token);
}