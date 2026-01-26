using Microsoft.EntityFrameworkCore;
using SecureFileDelivery.Application.Interfaces;
using SecureFileDelivery.Domain.Entities;
using SecureFileDelivery.Infrastructure.Data;

namespace SecureFileDelivery.Infrastructure.Repositories;

public class StatementRepository : IStatementRepository
{
    private readonly AppDbContext _context;

    public StatementRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Statement?> GetByIdAsync(Guid id)
    {
        return await _context.Statements
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Statement?> GetByIdWithLinksAsync(Guid id)
    {
        return await _context.Statements
            .Include(s => s.DownloadLinks)
            .Include(s => s.AuditLogs)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Statement>> GetByAccountNumberAsync(string accountNumber)
    {
        return await _context.Statements
            .Where(s => s.AccountNumber == accountNumber)
            .OrderByDescending(s => s.UploadedAt)
            .ToListAsync();
    }
    public async Task<Statement> AddAsync(Statement statement)
    {
        await _context.Statements.AddAsync(statement);
        await _context.SaveChangesAsync();
        return statement;
    }

    public async Task UpdateAsync(Statement statement)
    {
        _context.Statements.Update(statement);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var statement = await GetByIdAsync(id);
        if (statement != null)
        {
            _context.Statements.Remove(statement);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Statements.AnyAsync(s => s.Id == id);
    }
}