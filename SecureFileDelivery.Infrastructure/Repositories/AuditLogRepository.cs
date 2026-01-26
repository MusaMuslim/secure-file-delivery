using Microsoft.EntityFrameworkCore;
using SecureFileDelivery.Application.Interfaces;
using SecureFileDelivery.Domain.Entities;
using SecureFileDelivery.Infrastructure.Data;

namespace SecureFileDelivery.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id)
    {
        return await _context.AuditLogs
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AuditLog>> GetByStatementIdAsync(Guid statementId)
    {
        return await _context.AuditLogs
            .Where(a => a.StatementId == statementId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, int limit = 100)
    {
        return await _context.AuditLogs
            .Where(a => a.AccessedBy == userId)
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<AuditLog> AddAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
        return log;
    }
}