// File: Ogur.Hub.Infrastructure/Persistence/Configurations/TelemetryConfiguration.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence.Configurations

using Ogur.Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogur.Hub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Telemetry entity.
/// </summary>
public sealed class TelemetryConfiguration : IEntityTypeConfiguration<Telemetry>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Telemetry> builder)
    {
        builder.ToTable("Telemetries");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.EventDataJson)
            .HasColumnType("json");

        builder.Property(t => t.OccurredAt)
            .IsRequired();

        builder.Property(t => t.ReceivedAt)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        builder.HasIndex(t => new { t.DeviceId, t.EventType, t.ReceivedAt });

        builder.HasIndex(t => t.ReceivedAt);
    }
}