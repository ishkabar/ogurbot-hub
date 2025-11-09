// File: Ogur.Hub.Infrastructure/Persistence/ApplicationDbContext.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ogur.Hub.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for Ogur Hub application.
/// </summary>
public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">Database context options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    /// <inheritdoc />
    public DbSet<Domain.Entities.Application> Applications => Set<Domain.Entities.Application>();

    /// <inheritdoc />
    public DbSet<User> Users => Set<User>();

    /// <inheritdoc />
    public DbSet<License> Licenses => Set<License>();

    /// <inheritdoc />
    public DbSet<Device> Devices => Set<Device>();

    /// <inheritdoc />
    public DbSet<DeviceSession> DeviceSessions => Set<DeviceSession>();

    /// <inheritdoc />
    public DbSet<ApplicationVersion> ApplicationVersions => Set<ApplicationVersion>();

    /// <inheritdoc />
    public DbSet<Telemetry> Telemetry => Set<Telemetry>();

    /// <inheritdoc />
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <inheritdoc />
    public DbSet<HubCommand> HubCommands => Set<HubCommand>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}