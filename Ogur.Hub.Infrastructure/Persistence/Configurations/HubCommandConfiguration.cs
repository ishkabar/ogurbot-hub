// File: Hub.Infrastructure/Persistence/Configurations/HubCommandConfiguration.cs
// Project: Hub.Infrastructure
// Namespace: Hub.Infrastructure.Persistence.Configurations

using Ogur.Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogur.Hub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for HubCommand entity.
/// </summary>
public sealed class HubCommandConfiguration : IEntityTypeConfiguration<HubCommand>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<HubCommand> builder)
    {
        builder.ToTable("HubCommands");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CommandType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.Payload)
            .IsRequired()
            .HasColumnType("json");

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.SentAt)
            .IsRequired();

        builder.Property(c => c.AcknowledgedAt);

        builder.Property(c => c.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        builder.HasIndex(c => new { c.DeviceId, c.Status, c.SentAt });
        
        builder.Property(c => c.ClientGuid)
            .IsRequired();

        builder.HasIndex(c => c.ClientGuid)
            .IsUnique();
    }
}