// File: Ogur.Hub.Application/Interfaces/IVpsRepository.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Interfaces

using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Application.Interfaces;

/// <summary>
/// Repository interface for VPS-related entities.
/// </summary>
public interface IVpsRepository
{
    /// <summary>
    /// Gets all Docker containers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of containers.</returns>
    Task<List<VpsContainer>> GetAllContainersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a container by Docker container ID.
    /// </summary>
    /// <param name="containerId">Docker container ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Container if found, null otherwise.</returns>
    Task<VpsContainer?> GetContainerByDockerIdAsync(string containerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new container.
    /// </summary>
    /// <param name="container">Container to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task AddContainerAsync(VpsContainer container, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing container.
    /// </summary>
    /// <param name="container">Container to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task UpdateContainerAsync(VpsContainer container, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all websites.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of websites.</returns>
    Task<List<VpsWebsite>> GetAllWebsitesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a website by ID.
    /// </summary>
    /// <param name="id">Website ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Website if found, null otherwise.</returns>
    Task<VpsWebsite?> GetWebsiteByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new website.
    /// </summary>
    /// <param name="website">Website to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task AddWebsiteAsync(VpsWebsite website, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing website.
    /// </summary>
    /// <param name="website">Website to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task UpdateWebsiteAsync(VpsWebsite website, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a website.
    /// </summary>
    /// <param name="website">Website to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task DeleteWebsiteAsync(VpsWebsite website, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all resource snapshots.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of snapshots.</returns>
    Task<List<VpsResourceSnapshot>> GetAllSnapshotsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new resource snapshot.
    /// </summary>
    /// <param name="snapshot">Snapshot to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task AddSnapshotAsync(VpsResourceSnapshot snapshot, CancellationToken cancellationToken = default);
}