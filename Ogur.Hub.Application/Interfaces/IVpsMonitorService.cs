// File: Ogur.Hub.Application/Interfaces/IVpsMonitorService.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Interfaces

using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Application.Interfaces;

/// <summary>
/// Service interface for VPS monitoring operations.
/// </summary>
public interface IVpsMonitorService
{
    /// <summary>
    /// Gets all Docker containers with their current stats.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of containers with stats.</returns>
    Task<List<VpsContainerDto>> GetContainersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all configured websites.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of websites.</returns>
    Task<List<VpsWebsiteDto>> GetWebsitesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current VPS resource usage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Current resource snapshot.</returns>
    Task<VpsResourceDto> GetCurrentResourcesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets historical resource snapshots within a time range.
    /// </summary>
    /// <param name="from">Start timestamp.</param>
    /// <param name="to">End timestamp.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of resource snapshots.</returns>
    Task<List<VpsResourceDto>> GetResourceHistoryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates container statistics from Docker.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task RefreshContainerStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates website health checks.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task RefreshWebsiteHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures and stores current VPS resource snapshot.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task CaptureResourceSnapshotAsync(CancellationToken cancellationToken = default);
}