// File: Ogur.Hub.Application/Common/Interfaces/IApplicationDbContext.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Common.Interfaces

using Microsoft.EntityFrameworkCore;
using Ogur.Hub.Domain.Entities;
using ApplicationEntity = Ogur.Hub.Domain.Entities.Application;


namespace Ogur.Hub.Application.Common.Interfaces;

/// <summary>
/// Database context interface for accessing entities.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the Applications DbSet.
    /// </summary>
    DbSet<ApplicationEntity> Applications { get; }

    /// <summary>
    /// Gets the Licenses DbSet.
    /// </summary>
    DbSet<License> Licenses { get; }

    /// <summary>
    /// Gets the Devices DbSet.
    /// </summary>
    DbSet<Device> Devices { get; }

    /// <summary>
    /// Gets the DeviceSessions DbSet.
    /// </summary>
    DbSet<DeviceSession> DeviceSessions { get; }

    /// <summary>
    /// Gets the Users DbSet.
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Gets the HubCommands DbSet.
    /// </summary>
    DbSet<HubCommand> HubCommands { get; }

    /// <summary>
    /// Gets the Telemetry DbSet.
    /// </summary>
    DbSet<Telemetry> Telemetry { get; }

    /// <summary>
    /// Gets the AuditLogs DbSet.
    /// </summary>
    DbSet<AuditLog> AuditLogs { get; }

    /// <summary>
    /// Gets the ApplicationVersions DbSet.
    /// </summary>
    DbSet<ApplicationVersion> ApplicationVersions { get; }

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Number of affected rows.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}