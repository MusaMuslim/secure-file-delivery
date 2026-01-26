using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SecureFileDelivery.Infrastructure.Data;

// Factory for creating DbContext at design time (for migrations)
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Use SQLite for migrations
        optionsBuilder.UseSqlite("Data Source=securefiledelivery.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}