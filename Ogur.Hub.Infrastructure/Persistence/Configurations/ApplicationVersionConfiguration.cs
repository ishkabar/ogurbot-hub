// File: Hub.Infrastructure/Persistence/Configurations/ApplicationVersionConfiguration.cs
// Project: Hub.Infrastructure
// Namespace: Hub.Infrastructure.Persistence.Configurations

using Ogur.Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogur.Hub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for ApplicationVersion entity.
/// </summary>
public sealed class ApplicationVersionConfiguration : IEntityTypeConfiguration<ApplicationVersion>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ApplicationVersion> builder)
    {
        builder.ToTable("ApplicationVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.ReleaseNotes)
            .HasColumnType("text");

        builder.Property(v => v.DownloadUrl)
            .HasMaxLength(1000);

        builder.Property(v => v.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(v => v.ReleasedAt)
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .IsRequired();

        builder.HasIndex(v => new { v.ApplicationId, v.Version })
            .IsUnique();

        builder.HasIndex(v => new { v.ApplicationId, v.ReleasedAt });
    }
}