// File: Hub.Infrastructure/Persistence/Configurations/DeviceSessionConfiguration.cs
// Project: Hub.Infrastructure
// Namespace: Hub.Infrastructure.Persistence.Configurations

using Ogur.Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogur.Hub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for DeviceSession entity.
/// </summary>
public sealed class DeviceSessionConfiguration : IEntityTypeConfiguration<DeviceSession>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DeviceSession> builder)
    {
        builder.ToTable("DeviceSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ConnectionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.IpAddress)
            .HasMaxLength(45);

        builder.Property(s => s.UserAgent)
            .HasMaxLength(500);

        builder.Property(s => s.ConnectedAt)
            .IsRequired();

        builder.Property(s => s.DisconnectedAt);

        builder.Property(s => s.LastHeartbeatAt);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        builder.HasIndex(s => s.ConnectionId)
            .IsUnique();

        builder.HasIndex(s => new { s.DeviceId, s.ConnectedAt });
    }
}