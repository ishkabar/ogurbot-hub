// File: Ogur.Hub.Infrastructure/Persistence/Configurations/DeviceUserConfiguration.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence.Configurations

using Ogur.Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogur.Hub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for DeviceUser entity.
/// </summary>
public sealed class DeviceUserConfiguration : IEntityTypeConfiguration<DeviceUser>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DeviceUser> builder)
    {
        builder.ToTable("DeviceUsers");

        builder.HasKey(du => du.Id);

        builder.Property(du => du.Id)
            .ValueGeneratedOnAdd();

        builder.Property(du => du.AssignedAt)
            .IsRequired();

        builder.Property(du => du.CreatedAt)
            .IsRequired();

        builder.Property(du => du.UpdatedAt)
            .IsRequired();

        builder.HasOne(du => du.Device)
            .WithMany(d => d.DeviceUsers)
            .HasForeignKey(du => du.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(du => du.User)
            .WithMany()
            .HasForeignKey(du => du.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(du => new { du.DeviceId, du.UserId })
            .IsUnique();
    }
}