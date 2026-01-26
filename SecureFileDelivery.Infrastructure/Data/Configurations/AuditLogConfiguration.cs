using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureFileDelivery.Domain.Entities;

namespace SecureFileDelivery.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        // Table name
        builder.ToTable("AuditLogs");

        // Primary key
        builder.HasKey(a => a.Id);

        // Properties
        builder.Property(a => a.AccessedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // IPv6 max length

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Details)
            .HasMaxLength(1000);

        // Indexes for performance (audit logs are queried frequently)
        builder.HasIndex(a => a.StatementId);
        builder.HasIndex(a => a.Timestamp);
        builder.HasIndex(a => a.AccessedBy);
        builder.HasIndex(a => a.Action);
    }
}