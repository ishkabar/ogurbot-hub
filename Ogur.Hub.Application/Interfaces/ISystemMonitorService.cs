// File: Ogur.Hub.Application/Interfaces/ISystemMonitorService.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Interfaces

namespace Ogur.Hub.Application.Interfaces;

/// <summary>
/// Service interface for system resource monitoring.
/// </summary>
public interface ISystemMonitorService
{
    /// <summary>
    /// Captures and stores current system resource snapshot.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task CaptureSnapshotAsync(CancellationToken cancellationToken = default);
}