using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureFileDelivery.Domain.Enums;

namespace SecureFileDelivery.Domain.Entities;

// Represents a customer bank statement file stored in the system
public class Statement
{
    // Unique identifier for the statement
    public Guid Id { get; private set; }
    
    // Customer account number this statement belongs to
    public string AccountNumber { get; private set; }
    
    // Original filename of the uploaded statement
    public string FileName { get; private set; }
    
    // Type of file (PDF, CSV, Excel, etc.)
    public FileType FileType { get; private set; }
    
    // File size in bytes
    public long FileSizeBytes { get; private set; }
    
    // Path where the encrypted file is stored
    public string StoragePath { get; private set; }
    
    // Encryption key identifier (for security)
    public string EncryptionKeyId { get; private set; }
    
    // Statement period start date
    public DateTime PeriodStart { get; private set; }
    
    // Statement period end date
    public DateTime PeriodEnd { get; private set; }
    
    // When this statement was uploaded to the system
    public DateTime UploadedAt { get; private set; }
    
    // Who uploaded this statement (user/system identifier)
    public string UploadedBy { get; private set; }
    
    // Download links generated for this statement
    public ICollection<DownloadLink> DownloadLinks { get; private set; }
    
    // Audit logs tracking access to this statement
    public ICollection<AuditLog> AuditLogs { get; private set; }

    // Private constructor for EF Core
    private Statement()
    {
        DownloadLinks = new List<DownloadLink>();
        AuditLogs = new List<AuditLog>();
    }

    // Creates a new statement entity
    public Statement(
        string accountNumber,
        string fileName,
        FileType fileType,
        long fileSizeBytes,
        string storagePath,
        string encryptionKeyId,
        DateTime periodStart,
        DateTime periodEnd,
        string uploadedBy)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number cannot be empty", nameof(accountNumber));
        
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));
        
        if (fileSizeBytes <= 0)
            throw new ArgumentException("File size must be positive", nameof(fileSizeBytes));
        
        if (periodStart >= periodEnd)
            throw new ArgumentException("Period start must be before period end");

        Id = Guid.NewGuid();
        AccountNumber = accountNumber;
        FileName = fileName;
        FileType = fileType;
        FileSizeBytes = fileSizeBytes;
        StoragePath = storagePath;
        EncryptionKeyId = encryptionKeyId;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        UploadedAt = DateTime.UtcNow;
        UploadedBy = uploadedBy;
        DownloadLinks = new List<DownloadLink>();
        AuditLogs = new List<AuditLog>();
    }

    // Creates a download link for this statement
    public DownloadLink CreateDownloadLink(int expirationMinutes, string createdBy)
    {
        var link = new DownloadLink(Id, expirationMinutes, createdBy);
        DownloadLinks.Add(link);
        return link;
    }

    // Records an access audit log
    public void RecordAccess(string accessedBy, string ipAddress, string action)
    {
        var log = new AuditLog(Id, accessedBy, ipAddress, action);
        AuditLogs.Add(log);
    }
}