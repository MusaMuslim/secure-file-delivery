using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureFileDelivery.Domain.Entities;

// Tracks all access and operations performed on statements
public class AuditLog
{
    // Unique identifier for this audit entry
    public Guid Id { get; private set; }

    // The statement that was accessed
    public Guid StatementId { get; private set; }

    // Who performed the action (user ID, system account, etc.)
    public string AccessedBy { get; private set; }

    // IP address of the accessor (for security tracking)
    public string IpAddress { get; private set; }

    // What action was performed (e.g., "Downloaded", "Link Created", "Link Revoked")
    public string Action { get; private set; }

    // When this action occurred
    public DateTime Timestamp { get; private set; }

    // Additional context or metadata (JSON format)
    public string? Details { get; private set; }

    // Navigation property to the statement
    public Statement Statement { get; private set; }

    // Private constructor for EF Core
    private AuditLog()
    {
    }

    // Creates a new audit log entry
    public AuditLog(
        Guid statementId,
        string accessedBy,
        string ipAddress,
        string action,
        string? details = null)
    {
        if (string.IsNullOrWhiteSpace(accessedBy))
            throw new ArgumentException("Accessed by cannot be empty", nameof(accessedBy));

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));

        Id = Guid.NewGuid();
        StatementId = statementId;
        AccessedBy = accessedBy;
        IpAddress = ipAddress ?? "Unknown";
        Action = action;
        Timestamp = DateTime.UtcNow;
        Details = details;
    }

    // Creates an audit log for file upload
    public static AuditLog ForUpload(Guid statementId, string uploadedBy, string ipAddress, string fileName)
    {
        return new AuditLog(
            statementId,
            uploadedBy,
            ipAddress,
            "Statement Uploaded",
            $"File: {fileName}");
    }

    // Creates an audit log for file download
    public static AuditLog ForDownload(Guid statementId, string downloadedBy, string ipAddress, string linkToken)
    {
        return new AuditLog(
            statementId,
            downloadedBy,
            ipAddress,
            "Statement Downloaded",
            $"Link Token: {linkToken}");
    }

    // Creates an audit log for link creation
    public static AuditLog ForLinkCreation(Guid statementId, string createdBy, string ipAddress, int expirationMinutes)
    {
        return new AuditLog(
            statementId,
            createdBy,
            ipAddress,
            "Download Link Created",
            $"Expires in: {expirationMinutes} minutes");
    }
}