// File: Ogur.Hub.Application/DTO/VpsContainerDto.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.DTO

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// Data transfer object for VPS container information.
/// </summary>
public record VpsContainerDto
{
    /// <summary>
    /// Gets the container identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the Docker container ID.
    /// </summary>
    public string ContainerId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the container name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the Docker image name.
    /// </summary>
    public string Image { get; init; } = string.Empty;

    /// <summary>
    /// Gets the container state.
    /// </summary>
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// Gets the container status message.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the timestamp when the container was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when the container was started.
    /// </summary>
    public DateTime? StartedAt { get; init; }

    /// <summary>
    /// Gets the CPU usage percentage.
    /// </summary>
    public decimal CpuUsagePercent { get; init; }

    /// <summary>
    /// Gets the memory usage in megabytes.
    /// </summary>
    public decimal MemoryUsageMb { get; init; }

    /// <summary>
    /// Gets the memory limit in megabytes.
    /// </summary>
    public decimal MemoryLimitMb { get; init; }

    /// <summary>
    /// Gets the memory usage percentage.
    /// </summary>
    public decimal MemoryUsagePercent { get; init; }

    /// <summary>
    /// Gets the network received in megabytes.
    /// </summary>
    public decimal NetworkRxMb { get; init; }

    /// <summary>
    /// Gets the network transmitted in megabytes.
    /// </summary>
    public decimal NetworkTxMb { get; init; }

    /// <summary>
    /// Gets the timestamp when stats were last updated.
    /// </summary>
    public DateTime LastUpdatedAt { get; init; }
}