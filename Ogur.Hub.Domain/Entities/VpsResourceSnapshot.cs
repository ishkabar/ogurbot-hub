// File: Ogur.Hub.Domain/Entities/VpsResourceSnapshot.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a snapshot of VPS system resources at a point in time.
/// </summary>
public class VpsResourceSnapshot
{
    /// <summary>
    /// Gets or sets the snapshot identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the snapshot was taken.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the CPU usage percentage.
    /// </summary>
    public decimal CpuUsagePercent { get; set; }

    /// <summary>
    /// Gets or sets the total memory in bytes.
    /// </summary>
    public long MemoryTotalBytes { get; set; }

    /// <summary>
    /// Gets or sets the used memory in bytes.
    /// </summary>
    public long MemoryUsedBytes { get; set; }

    /// <summary>
    /// Gets or sets the total disk space in bytes.
    /// </summary>
    public long DiskTotalBytes { get; set; }

    /// <summary>
    /// Gets or sets the used disk space in bytes.
    /// </summary>
    public long DiskUsedBytes { get; set; }

    /// <summary>
    /// Gets or sets the network received bytes per second.
    /// </summary>
    public long NetworkRxBytesPerSec { get; set; }

    /// <summary>
    /// Gets or sets the network transmitted bytes per second.
    /// </summary>
    public long NetworkTxBytesPerSec { get; set; }

    /// <summary>
    /// Gets or sets the system load average (1 minute).
    /// </summary>
    public decimal LoadAverage1Min { get; set; }

    /// <summary>
    /// Gets or sets the system load average (5 minutes).
    /// </summary>
    public decimal LoadAverage5Min { get; set; }

    /// <summary>
    /// Gets or sets the system load average (15 minutes).
    /// </summary>
    public decimal LoadAverage15Min { get; set; }
}