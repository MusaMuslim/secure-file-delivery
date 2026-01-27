using SecureFileDelivery.Domain.Enums;

namespace SecureFileDelivery.Application.DTOs;

// Response DTO for statement information
public class StatementResponse
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public FileType FileType { get; set; }
    public long FileSizeBytes { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public int DownloadLinkCount { get; set; }
}