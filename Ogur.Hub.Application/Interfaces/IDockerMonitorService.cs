// File: Ogur.Hub.Application/Interfaces/IDockerMonitorService.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Interfaces

namespace Ogur.Hub.Application.Interfaces;

/// <summary>
/// Service interface for Docker container monitoring.
/// </summary>
public interface IDockerMonitorService
{
    /// <summary>
    /// Updates all container information from Docker.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task UpdateContainersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates container statistics from Docker.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task UpdateContainerStatsAsync(CancellationToken cancellationToken = default);
}