using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureFileDelivery.Domain.Enums;
using SecureFileDelivery.Domain.Exceptions;

namespace SecureFileDelivery.Domain.Entities;

// Represents a secure, time-limited download link for a statement
public class DownloadLink
{
    // Unique identifier for the download link
    public Guid Id { get; private set; }

    // The statement this link provides access to
    public Guid StatementId { get; private set; }

    // Unique token used in the download URL
    public string Token { get; private set; }

    // Current status of the link
    public LinkStatus Status { get; private set; }

    // When this link was created
    public DateTime CreatedAt { get; private set; }

    // Who created this link
    public string CreatedBy { get; private set; }

    // When this link expires
    public DateTime ExpiresAt { get; private set; }

    // When this link was first accessed (if ever)
    public DateTime? AccessedAt { get; private set; }

    // How many times this link has been accessed
    public int AccessCount { get; private set; }

    // Maximum number of times this link can be used (null = unlimited)
    public int? MaxAccessCount { get; private set; }

    // Navigation property to the statement
    public Statement Statement { get; private set; }

    // Private constructor for EF Core
    private DownloadLink()
    {
    }

    // Creates a new download link
    public DownloadLink(Guid statementId, int expirationMinutes, string createdBy, int? maxAccessCount = null)
    {
        if (expirationMinutes <= 0)
            throw new ArgumentException("Expiration must be positive", nameof(expirationMinutes));

        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by cannot be empty", nameof(createdBy));

        Id = Guid.NewGuid();
        StatementId = statementId;
        Token = GenerateSecureToken();
        Status = LinkStatus.Active;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
        AccessCount = 0;
        MaxAccessCount = maxAccessCount;
    }

    // Validates if this link can be used
    public void ValidateAccess()
    {
        if (Status == LinkStatus.Revoked)
            throw new InvalidLinkException("This download link has been revoked");

        if (Status == LinkStatus.Expired || DateTime.UtcNow > ExpiresAt)
        {
            Status = LinkStatus.Expired;
            throw new InvalidLinkException("This download link has expired");
        }

        if (MaxAccessCount.HasValue && AccessCount >= MaxAccessCount.Value)
        {
            Status = LinkStatus.Expired;
            throw new InvalidLinkException("This download link has reached its maximum access count");
        }
    }

    // Records an access to this link
    public void RecordAccess()
    {
        ValidateAccess();

        AccessCount++;
        AccessedAt ??= DateTime.UtcNow;
        Status = LinkStatus.Used;

        // Auto-expire if max access reached
        if (MaxAccessCount.HasValue && AccessCount >= MaxAccessCount.Value)
        {
            Status = LinkStatus.Expired;
        }
    }

    // Manually revokes this link
    public void Revoke()
    {
        Status = LinkStatus.Revoked;
    }

    // Generates a cryptographically secure random token
    private static string GenerateSecureToken()
    {
        // Generate 32 random bytes and convert to base64 URL-safe string
        var bytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}