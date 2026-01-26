using Microsoft.EntityFrameworkCore;
using SecureFileDelivery.Domain.Entities;

namespace SecureFileDelivery.Infrastructure.Data;

// Database context for the Secure File Delivery system
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets which represent tables in the database
    public DbSet<Statement> Statements { get; set; }
    public DbSet<DownloadLink> DownloadLinks { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}