// File: Ogur.Hub.Infrastructure/Persistence/Configurations/ApplicationConfiguration.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence.Configurations

using Ogur.Hub.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogur.Hub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Application entity.
/// </summary>
public sealed class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Entities.Application>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Domain.Entities.Application> builder)
    {
        builder.ToTable("Applications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.CurrentVersion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.ApiKey)
            .IsRequired()
            .HasMaxLength(128)
            .HasConversion<ApiKeyConverter>();

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.HasIndex(a => a.Name)
            .IsUnique();
    }
}