// File: Hub.Infrastructure/Persistence/Configurations/LicenseConfiguration.cs
// Project: Hub.Infrastructure
// Namespace: Hub.Infrastructure.Persistence.Configurations

using Ogur.Hub.Domain.Entities;
using Ogur.Hub.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogur.Hub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for License entity.
/// </summary>
public sealed class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ToTable("Licenses");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.LicenseKey)
            .IsRequired()
            .HasMaxLength(64)
            .HasConversion<LicenseKeyConverter>();

        builder.Property(l => l.MaxDevices)
            .IsRequired()
            .HasDefaultValue(2);

        builder.Property(l => l.StartDate)
            .IsRequired();

        builder.Property(l => l.EndDate);

        builder.Property(l => l.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .IsRequired();

        builder.HasIndex(l => l.LicenseKey)
            .IsUnique();

        builder.HasIndex(l => new { l.ApplicationId, l.UserId });

        builder.HasMany(l => l.Devices)
            .WithOne(d => d.License)
            .HasForeignKey(d => d.LicenseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}