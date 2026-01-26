using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureFileDelivery.Domain.Entities;

namespace SecureFileDelivery.Infrastructure.Data.Configurations;

public class StatementConfiguration : IEntityTypeConfiguration<Statement>
{
    public void Configure(EntityTypeBuilder<Statement> builder)
    {
        // Table name
        builder.ToTable("Statements");

        // Primary key
        builder.HasKey(s => s.Id);

        // Properties
        builder.Property(s => s.AccountNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.FileType)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string

        builder.Property(s => s.FileSizeBytes)
            .IsRequired();

        builder.Property(s => s.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.EncryptionKeyId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.UploadedBy)
            .IsRequired()
            .HasMaxLength(100);

        // Relationships
        builder.HasMany(s => s.DownloadLinks)
            .WithOne(d => d.Statement)
            .HasForeignKey(d => d.StatementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.AuditLogs)
            .WithOne(a => a.Statement)
            .HasForeignKey(a => a.StatementId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(s => s.AccountNumber);
        builder.HasIndex(s => s.UploadedAt);
    }
}