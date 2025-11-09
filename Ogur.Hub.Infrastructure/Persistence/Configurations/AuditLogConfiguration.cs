// File: Ogur.Hub.Infrastructure/Persistence/Configurations/AuditLogConfiguration.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence.Configurations

using Ogur.Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogur.Hub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for AuditLog entity.
/// </summary>
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityId);

        builder.Property(a => a.DetailsJson)
            .HasColumnType("json");

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45);

        builder.Property(a => a.OccurredAt)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        builder.HasIndex(a => new { a.UserId, a.OccurredAt });

        builder.HasIndex(a => new { a.EntityType, a.EntityId });

        builder.HasIndex(a => a.OccurredAt);
    }
}