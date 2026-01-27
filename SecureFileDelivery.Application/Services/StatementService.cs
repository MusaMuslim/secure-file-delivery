using SecureFileDelivery.Application.DTOs;
using SecureFileDelivery.Application.Interfaces;
using SecureFileDelivery.Domain.Entities;
using SecureFileDelivery.Domain.Enums;
using SecureFileDelivery.Domain.Exceptions;

namespace SecureFileDelivery.Application.Services;

public class StatementService : IStatementService
{
    private readonly IStatementRepository _statementRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IEncryptionService _encryptionService;
    private readonly IAuditLogRepository _auditLogRepository;

    public StatementService(
        IStatementRepository statementRepository,
        IFileStorageService fileStorageService,
        IEncryptionService encryptionService,
        IAuditLogRepository auditLogRepository)
    {
        _statementRepository = statementRepository;
        _fileStorageService = fileStorageService;
        _encryptionService = encryptionService;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<StatementResponse> UploadStatementAsync(UploadStatementRequest request)
    {
        // Validate file
        if (request.File == null || request.File.Length == 0)
            throw new InvalidFileException("File is required");

        // Determine file type
        var fileType = GetFileTypeFromExtension(Path.GetExtension(request.File.FileName));

        // Generate encryption key ID
        var encryptionKeyId = _encryptionService.GenerateKeyId();

        // Encrypt file
        await using var fileStream = request.File.OpenReadStream();
        await using var encryptedStream = await _encryptionService.EncryptAsync(fileStream, encryptionKeyId);

        // Save encrypted file
        var storagePath = await _fileStorageService.SaveFileAsync(encryptedStream, request.File.FileName);

        // Create statement entity
        var statement = new Statement(
            request.AccountNumber,
            request.File.FileName,
            fileType,
            request.File.Length,
            storagePath,
            encryptionKeyId,
            request.PeriodStart,
            request.PeriodEnd,
            request.UploadedBy
        );

        // Save to database
        await _statementRepository.AddAsync(statement);

        // Create audit log
        var auditLog = AuditLog.ForUpload(
            statement.Id,
            request.UploadedBy,
            "System", // IP address would come from HTTP context
            request.File.FileName
        );
        await _auditLogRepository.AddAsync(auditLog);

        // Return response
        return MapToResponse(statement);
    }

    public async Task<StatementResponse?> GetStatementByIdAsync(Guid id)
    {
        var statement = await _statementRepository.GetByIdWithLinksAsync(id);
        return statement == null ? null : MapToResponse(statement);
    }

    public async Task<IEnumerable<StatementResponse>> GetStatementsByAccountAsync(string accountNumber)
    {
        var statements = await _statementRepository.GetByAccountNumberAsync(accountNumber);
        return statements.Select(MapToResponse);
    }

    public async Task DeleteStatementAsync(Guid id)
    {
        var statement = await _statementRepository.GetByIdAsync(id);
        if (statement == null)
            throw new EntityNotFoundException(nameof(Statement), id);

        // Delete file from storage
        await _fileStorageService.DeleteFileAsync(statement.StoragePath);

        // Delete from database
        await _statementRepository.DeleteAsync(id);
    }

    private static StatementResponse MapToResponse(Statement statement)
    {
        return new StatementResponse
        {
            Id = statement.Id,
            AccountNumber = statement.AccountNumber,
            FileName = statement.FileName,
            FileType = statement.FileType,
            FileSizeBytes = statement.FileSizeBytes,
            PeriodStart = statement.PeriodStart,
            PeriodEnd = statement.PeriodEnd,
            UploadedAt = statement.UploadedAt,
            UploadedBy = statement.UploadedBy,
            DownloadLinkCount = statement.DownloadLinks?.Count ?? 0
        };
    }

    private static FileType GetFileTypeFromExtension(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".pdf" => FileType.Pdf,
            ".csv" => FileType.Csv,
            ".xlsx" or ".xls" => FileType.Excel,
            _ => FileType.Other
        };
    }
}