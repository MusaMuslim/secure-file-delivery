using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureFileDelivery.Domain.Entities;

namespace SecureFileDelivery.Infrastructure.Data.Configurations;

public class DownloadLinkConfiguration : IEntityTypeConfiguration<DownloadLink>
{
    public void Configure(EntityTypeBuilder<DownloadLink> builder)
    {
        // Table name
        builder.ToTable("DownloadLinks");

        // Primary key
        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.Token)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string

        builder.Property(d => d.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.AccessCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes for performance
        builder.HasIndex(d => d.Token)
            .IsUnique(); // Token must be unique

        builder.HasIndex(d => d.StatementId);
        builder.HasIndex(d => d.ExpiresAt);
        builder.HasIndex(d => d.Status);
    }
}