using SecureFileDelivery.Domain.Enums;

namespace SecureFileDelivery.Application.DTOs;

// Response DTO for download link information
public class DownloadLinkResponse
{
    public Guid Id { get; set; }
    public Guid StatementId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public LinkStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int AccessCount { get; set; }
    public int? MaxAccessCount { get; set; }
    public bool IsExpired => DateTime.UtcNow > ExpiresAt || Status == LinkStatus.Expired;
}