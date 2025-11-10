// File: Ogur.Hub.Application/DTO/VpsResourceDto.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.DTO

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// Data transfer object for VPS resource information.
/// </summary>
public record VpsResourceDto
{
    /// <summary>
    /// Gets the CPU usage percentage.
    /// </summary>
    public decimal CpuUsagePercent { get; init; }

    /// <summary>
    /// Gets the total memory in gigabytes.
    /// </summary>
    public decimal MemoryTotalGb { get; init; }

    /// <summary>
    /// Gets the used memory in gigabytes.
    /// </summary>
    public decimal MemoryUsedGb { get; init; }

    /// <summary>
    /// Gets the memory usage percentage.
    /// </summary>
    public decimal MemoryUsagePercent { get; init; }

    /// <summary>
    /// Gets the total disk space in gigabytes.
    /// </summary>
    public decimal DiskTotalGb { get; init; }

    /// <summary>
    /// Gets the used disk space in gigabytes.
    /// </summary>
    public decimal DiskUsedGb { get; init; }

    /// <summary>
    /// Gets the disk usage percentage.
    /// </summary>
    public decimal DiskUsagePercent { get; init; }

    /// <summary>
    /// Gets the network received rate in megabytes per second.
    /// </summary>
    public decimal NetworkRxMbps { get; init; }

    /// <summary>
    /// Gets the network transmitted rate in megabytes per second.
    /// </summary>
    public decimal NetworkTxMbps { get; init; }

    /// <summary>
    /// Gets the system load average (1 minute).
    /// </summary>
    public decimal LoadAverage1Min { get; init; }

    /// <summary>
    /// Gets the system load average (5 minutes).
    /// </summary>
    public decimal LoadAverage5Min { get; init; }

    /// <summary>
    /// Gets the system load average (15 minutes).
    /// </summary>
    public decimal LoadAverage15Min { get; init; }

    /// <summary>
    /// Gets the timestamp when the snapshot was taken.
    /// </summary>
    public DateTime Timestamp { get; init; }
}