namespace SecureFileDelivery.Application.DTOs;

// Request to generate a download link for a statement
public class GenerateLinkRequest
{
    // ID of the statement to create a link for
    public Guid StatementId { get; set; }

    // How many minutes until the link expires (default: 60)
    public int ExpirationMinutes { get; set; } = 60;

    // Maximum number of times this link can be accessed (null = unlimited)
    public int? MaxAccessCount { get; set; }

    // Who is creating this link
    public string CreatedBy { get; set; } = string.Empty;
}