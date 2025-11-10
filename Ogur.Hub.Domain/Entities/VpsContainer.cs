// File: Ogur.Hub.Domain/Entities/VpsContainer.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a Docker container running on the VPS.
/// </summary>
public class VpsContainer
{
    /// <summary>
    /// Gets or sets the container identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Docker container ID.
    /// </summary>
    public string ContainerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the container name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Docker image name.
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the container state (running, stopped, paused, etc).
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the container status message.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the container was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the container was started.
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets CPU usage percentage.
    /// </summary>
    public decimal CpuUsagePercent { get; set; }

    /// <summary>
    /// Gets or sets memory usage in bytes.
    /// </summary>
    public long MemoryUsageBytes { get; set; }

    /// <summary>
    /// Gets or sets memory limit in bytes.
    /// </summary>
    public long MemoryLimitBytes { get; set; }

    /// <summary>
    /// Gets or sets network received bytes.
    /// </summary>
    public long NetworkRxBytes { get; set; }

    /// <summary>
    /// Gets or sets network transmitted bytes.
    /// </summary>
    public long NetworkTxBytes { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the container stats were last updated.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }
}