using SecureFileDelivery.Domain.Entities;

namespace SecureFileDelivery.Application.Interfaces;

// Repository interface for Statement operations
public interface IStatementRepository
{
    Task<Statement?> GetByIdAsync(Guid id);
    Task<Statement?> GetByIdWithLinksAsync(Guid id);
    Task<IEnumerable<Statement>> GetByAccountNumberAsync(string accountNumber);
    Task<Statement> AddAsync(Statement statement);
    Task UpdateAsync(Statement statement);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

// Repository interface for DownloadLink operations
public interface IDownloadLinkRepository
{
    Task<DownloadLink?> GetByIdAsync(Guid id);
    Task<DownloadLink?> GetByTokenAsync(string token);
    Task<IEnumerable<DownloadLink>> GetByStatementIdAsync(Guid statementId);
    Task<DownloadLink> AddAsync(DownloadLink link);
    Task UpdateAsync(DownloadLink link);
    Task DeleteAsync(Guid id);
}

// Repository interface for AuditLog operations
public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(Guid id);
    Task<IEnumerable<AuditLog>> GetByStatementIdAsync(Guid statementId);
    Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, int limit = 100);
    Task<AuditLog> AddAsync(AuditLog log);
}