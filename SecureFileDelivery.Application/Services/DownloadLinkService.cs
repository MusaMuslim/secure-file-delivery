using SecureFileDelivery.Application.DTOs;
using SecureFileDelivery.Application.Interfaces;
using SecureFileDelivery.Domain.Entities;
using SecureFileDelivery.Domain.Exceptions;

namespace SecureFileDelivery.Application.Services;

public class DownloadLinkService : IDownloadLinkService
{
    private readonly IDownloadLinkRepository _linkRepository;
    private readonly IStatementRepository _statementRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IEncryptionService _encryptionService;
    private readonly IAuditLogRepository _auditLogRepository;

    public DownloadLinkService(
        IDownloadLinkRepository linkRepository,
        IStatementRepository statementRepository,
        IFileStorageService fileStorageService,
        IEncryptionService encryptionService,
        IAuditLogRepository auditLogRepository)
    {
        _linkRepository = linkRepository;
        _statementRepository = statementRepository;
        _fileStorageService = fileStorageService;
        _encryptionService = encryptionService;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<DownloadLinkResponse> GenerateLinkAsync(GenerateLinkRequest request, string baseUrl)
    {
        // Verify statement exists
        var statementExists = await _statementRepository.ExistsAsync(request.StatementId);
        if (!statementExists)
            throw new EntityNotFoundException(nameof(Statement), request.StatementId);

        // Create download link
        var link = new DownloadLink(
            request.StatementId,
            request.ExpirationMinutes,
            request.CreatedBy,
            request.MaxAccessCount
        );

        // Save to database
        await _linkRepository.AddAsync(link);

        // Create audit log
        var auditLog = AuditLog.ForLinkCreation(
            request.StatementId,
            request.CreatedBy,
            "System",
            request.ExpirationMinutes
        );
        await _auditLogRepository.AddAsync(auditLog);

        // Return response with full download URL
        return MapToResponse(link, baseUrl);
    }

    public async Task<DownloadLinkResponse> GetLinkByTokenAsync(string token)
    {
        var link = await _linkRepository.GetByTokenAsync(token);
        if (link == null)
            throw new InvalidLinkException("Download link not found");

        // Validate the link (throws if invalid)
        link.ValidateAccess();

        return MapToResponse(link, string.Empty);
    }

    public async Task<(Stream fileStream, string fileName, string contentType)> DownloadFileAsync(
        string token,
        string ipAddress)
    {
        // Get and validate link
        var link = await _linkRepository.GetByTokenAsync(token);
        if (link == null)
            throw new InvalidLinkException("Download link not found");

        // Validate link (throws if expired/revoked)
        link.ValidateAccess();

        // Get statement
        var statement = await _statementRepository.GetByIdAsync(link.StatementId);
        if (statement == null)
            throw new EntityNotFoundException(nameof(Statement), link.StatementId);

        // Record access
        link.RecordAccess();
        await _linkRepository.UpdateAsync(link);

        // Create audit log
        var auditLog = AuditLog.ForDownload(
            statement.Id,
            "Anonymous", // Could be user ID if authenticated
            ipAddress,
            token
        );
        await _auditLogRepository.AddAsync(auditLog);

        // Get encrypted file from storage
        var encryptedStream = await _fileStorageService.GetFileAsync(statement.StoragePath);

        // Decrypt file
        var decryptedStream = await _encryptionService.DecryptAsync(
            encryptedStream,
            statement.EncryptionKeyId
        );

        // Determine content type
        var contentType = GetContentType(statement.FileType);

        return (decryptedStream, statement.FileName, contentType);
    }

    public async Task<IEnumerable<DownloadLinkResponse>> GetLinksByStatementIdAsync(Guid statementId)
    {
        var links = await _linkRepository.GetByStatementIdAsync(statementId);
        return links.Select(l => MapToResponse(l, string.Empty));
    }

    public async Task RevokeLinkAsync(Guid linkId)
    {
        var link = await _linkRepository.GetByIdAsync(linkId);
        if (link == null)
            throw new EntityNotFoundException(nameof(DownloadLink), linkId);

        link.Revoke();
        await _linkRepository.UpdateAsync(link);
    }

    private static DownloadLinkResponse MapToResponse(DownloadLink link, string baseUrl)
    {
        var downloadUrl = string.IsNullOrEmpty(baseUrl)
            ? string.Empty
            : $"{baseUrl}/api/download/{link.Token}";

        return new DownloadLinkResponse
        {
            Id = link.Id,
            StatementId = link.StatementId,
            Token = link.Token,
            DownloadUrl = downloadUrl,
            Status = link.Status,
            CreatedAt = link.CreatedAt,
            ExpiresAt = link.ExpiresAt,
            AccessCount = link.AccessCount,
            MaxAccessCount = link.MaxAccessCount
        };
    }

    private static string GetContentType(Domain.Enums.FileType fileType)
    {
        return fileType switch
        {
            Domain.Enums.FileType.Pdf => "application/pdf",
            Domain.Enums.FileType.Csv => "text/csv",
            Domain.Enums.FileType.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }
}