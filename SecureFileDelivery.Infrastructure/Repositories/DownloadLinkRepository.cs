using Microsoft.EntityFrameworkCore;
using SecureFileDelivery.Application.Interfaces;
using SecureFileDelivery.Domain.Entities;
using SecureFileDelivery.Infrastructure.Data;

namespace SecureFileDelivery.Infrastructure.Repositories;

public class DownloadLinkRepository : IDownloadLinkRepository
{
    private readonly AppDbContext _context;

    public DownloadLinkRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DownloadLink?> GetByIdAsync(Guid id)
    {
        return await _context.DownloadLinks
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<DownloadLink?> GetByTokenAsync(string token)
    {
        return await _context.DownloadLinks
            .Include(d => d.Statement)
            .FirstOrDefaultAsync(d => d.Token == token);
    }

    public async Task<IEnumerable<DownloadLink>> GetByStatementIdAsync(Guid statementId)
    {
        return await _context.DownloadLinks
            .Where(d => d.StatementId == statementId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<DownloadLink> AddAsync(DownloadLink link)
    {
        await _context.DownloadLinks.AddAsync(link);
        await _context.SaveChangesAsync();
        return link;
    }

    public async Task UpdateAsync(DownloadLink link)
    {
        _context.DownloadLinks.Update(link);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var link = await GetByIdAsync(id);
        if (link != null)
        {
            _context.DownloadLinks.Remove(link);
            await _context.SaveChangesAsync();
        }
    }
}