using Microsoft.AspNetCore.Http;

namespace SecureFileDelivery.Application.DTOs;

// Request DTO for uploading a statement file
public class UploadStatementRequest
{
    // Customer account number
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// The file to upload
    /// </summary>
    public IFormFile File { get; set; } = null!;

    // Statement period start date
    public DateTime PeriodStart { get; set; }

    // Statement period end date
    public DateTime PeriodEnd { get; set; }

    // Who is uploading this statement
    public string UploadedBy { get; set; } = string.Empty;
}