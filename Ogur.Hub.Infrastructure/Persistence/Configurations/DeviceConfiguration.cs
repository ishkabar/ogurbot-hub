// File: Ogur.Hub.Infrastructure/Persistence/Configurations/DeviceConfiguration.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence.Configurations

using Ogur.Hub.Domain.Entities;
using Ogur.Hub.Domain.ValueObjects;
using Ogur.Hub.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogur.Hub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Device entity.
/// </summary>
public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Fingerprint)
            .IsRequired()
            .HasMaxLength(128)
            .HasConversion<DeviceFingerprintConverter>();

        builder.Property(d => d.DeviceName)
            .HasMaxLength(200);
            
            builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.LastIpAddress)
            .HasMaxLength(45);

        builder.Property(d => d.LastSeenAt);

        builder.Property(d => d.RegisteredAt)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired();
            
            builder.HasOne(d => d.License)
            .WithMany(l => l.Devices)
            .HasForeignKey(d => d.LicenseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.PrimaryUser)
            .WithMany()
            .HasForeignKey(d => d.PrimaryUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(d => new { d.LicenseId, d.Fingerprint })
            .IsUnique();
            
            builder.HasIndex(d => d.PrimaryUserId);

        builder.HasMany(d => d.Sessions)
            .WithOne(s => s.Device)
            .HasForeignKey(s => s.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Telemetries)
            .WithOne(t => t.Device)
            .HasForeignKey(t => t.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Commands)
            .WithOne(c => c.Device)
            .HasForeignKey(c => c.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(d => d.DeviceUsers)
            .WithOne(du => du.Device)
            .HasForeignKey(du => du.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}