// File: Ogur.Hub.Infrastructure/Persistence/ApplicationDbContext.Vps.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence

using Ogur.Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ogur.Hub.Infrastructure.Persistence;

/// <summary>
/// Partial class containing VPS-related entity configurations.
/// </summary>
public partial class ApplicationDbContext
{
    /// <summary>
    /// Gets or sets the VPS containers.
    /// </summary>
    public DbSet<VpsContainer> VpsContainers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the VPS websites.
    /// </summary>
    public DbSet<VpsWebsite> VpsWebsites { get; set; } = null!;

    /// <summary>
    /// Gets or sets the VPS resource snapshots.
    /// </summary>
    public DbSet<VpsResourceSnapshot> VpsResourceSnapshots { get; set; } = null!;

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        ConfigureVpsEntities(modelBuilder);
    }

    /// <summary>
    /// Configures VPS-related entities.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    private void ConfigureVpsEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VpsContainer>(entity =>
        {
            entity.ToTable("VpsContainers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContainerId).HasMaxLength(64).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Image).HasMaxLength(255).IsRequired();
            entity.Property(e => e.State).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(255).IsRequired();
            entity.Property(e => e.CpuUsagePercent).HasPrecision(5, 2);
            entity.HasIndex(e => e.ContainerId);
            entity.HasIndex(e => e.Name);
        });

        modelBuilder.Entity<VpsWebsite>(entity =>
        {
            entity.ToTable("VpsWebsites");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Domain).HasMaxLength(255).IsRequired();
            entity.Property(e => e.ServiceName).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Domain).IsUnique();
            entity.HasOne(e => e.Container)
                .WithMany()
                .HasForeignKey(e => e.ContainerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<VpsResourceSnapshot>(entity =>
        {
            entity.ToTable("VpsResourceSnapshots");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CpuUsagePercent).HasPrecision(5, 2);
            entity.Property(e => e.LoadAverage1Min).HasPrecision(5, 2);
            entity.Property(e => e.LoadAverage5Min).HasPrecision(5, 2);
            entity.Property(e => e.LoadAverage15Min).HasPrecision(5, 2);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}