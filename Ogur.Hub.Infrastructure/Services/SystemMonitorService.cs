// File: Ogur.Hub.Infrastructure/Services/SystemMonitorService.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Services

using System.Globalization;
using Ogur.Hub.Application.Interfaces;
using Ogur.Hub.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Ogur.Hub.Infrastructure.Services;

/// <summary>
/// Service for monitoring system resources (CPU, memory, disk, network).
/// </summary>
public class SystemMonitorService : ISystemMonitorService
{
    private readonly IVpsRepository _vpsRepository;
    private readonly ILogger<SystemMonitorService> _logger;
    private CpuStats? _previousCpuStats;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMonitorService"/> class.
    /// </summary>
    /// <param name="vpsRepository">VPS repository.</param>
    /// <param name="logger">Logger instance.</param>
    public SystemMonitorService(IVpsRepository vpsRepository, ILogger<SystemMonitorService> logger)
    {
        _vpsRepository = vpsRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task CaptureSnapshotAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cpuUsage = await GetCpuUsageAsync(cancellationToken);
            var memoryInfo = await GetMemoryInfoAsync(cancellationToken);
            var diskInfo = await GetDiskInfoAsync(cancellationToken);
            var loadAvg = await GetLoadAverageAsync(cancellationToken);

            var snapshot = new VpsResourceSnapshot
            {
                Timestamp = DateTime.UtcNow,
                CpuUsagePercent = cpuUsage,
                MemoryTotalBytes = memoryInfo.Total,
                MemoryUsedBytes = memoryInfo.Used,
                DiskTotalBytes = diskInfo.Total,
                DiskUsedBytes = diskInfo.Used,
                NetworkRxBytesPerSec = 0,
                NetworkTxBytesPerSec = 0,
                LoadAverage1Min = loadAvg.Load1,
                LoadAverage5Min = loadAvg.Load5,
                LoadAverage15Min = loadAvg.Load15
            };

            await _vpsRepository.AddSnapshotAsync(snapshot, cancellationToken);
            
            _logger.LogDebug("Captured resource snapshot: CPU={Cpu}%, Mem={Mem}GB/{Total}GB, Disk={Disk}GB/{DiskTotal}GB",
                cpuUsage, memoryInfo.Used / 1024m / 1024m / 1024m, memoryInfo.Total / 1024m / 1024m / 1024m,
                diskInfo.Used / 1024m / 1024m / 1024m, diskInfo.Total / 1024m / 1024m / 1024m);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to capture resource snapshot");
        }
    }

    private async Task<decimal> GetCpuUsageAsync(CancellationToken cancellationToken)
    {
        try
        {
            var procStatPath = "/host/proc/stat";
            if (!File.Exists(procStatPath))
            {
                _logger.LogWarning("/host/proc/stat not found, trying /proc/stat");
                procStatPath = "/proc/stat";
            }

            var lines = await File.ReadAllLinesAsync(procStatPath, cancellationToken);
            var cpuLine = lines.FirstOrDefault(l => l.StartsWith("cpu "));
            
            if (cpuLine == null)
            {
                _logger.LogWarning("CPU line not found in {Path}", procStatPath);
                return 0;
            }

            var parts = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5)
            {
                _logger.LogWarning("Invalid CPU line format in {Path}", procStatPath);
                return 0;
            }

            var currentStats = new CpuStats
            {
                User = long.Parse(parts[1]),
                Nice = long.Parse(parts[2]),
                System = long.Parse(parts[3]),
                Idle = long.Parse(parts[4]),
                IoWait = parts.Length > 5 ? long.Parse(parts[5]) : 0,
                Irq = parts.Length > 6 ? long.Parse(parts[6]) : 0,
                SoftIrq = parts.Length > 7 ? long.Parse(parts[7]) : 0
            };

            if (_previousCpuStats == null)
            {
                _previousCpuStats = currentStats;
                await Task.Delay(100, cancellationToken);
                return await GetCpuUsageAsync(cancellationToken);
            }

            var prevTotal = _previousCpuStats.Total;
            var prevIdle = _previousCpuStats.Idle;
            var currentTotal = currentStats.Total;
            var currentIdle = currentStats.Idle;

            var totalDiff = currentTotal - prevTotal;
            var idleDiff = currentIdle - prevIdle;

            _previousCpuStats = currentStats;

            if (totalDiff == 0) return 0;

            var usage = 100m * (totalDiff - idleDiff) / totalDiff;
            return Math.Round(usage, 2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read CPU usage");
            return 0;
        }
    }

    private async Task<MemoryInfo> GetMemoryInfoAsync(CancellationToken cancellationToken)
    {
        try
        {
            var meminfoPath = "/host/proc/meminfo";
            if (!File.Exists(meminfoPath))
            {
                _logger.LogWarning("/host/proc/meminfo not found, trying /proc/meminfo");
                meminfoPath = "/proc/meminfo";
            }

            var lines = await File.ReadAllLinesAsync(meminfoPath, cancellationToken);
            
            long memTotal = 0;
            long memAvailable = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("MemTotal:"))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && long.TryParse(parts[1], out var value))
                        memTotal = value * 1024;
                }
                else if (line.StartsWith("MemAvailable:"))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && long.TryParse(parts[1], out var value))
                        memAvailable = value * 1024;
                }

                if (memTotal > 0 && memAvailable > 0)
                    break;
            }

            var memUsed = memTotal - memAvailable;

            return new MemoryInfo
            {
                Total = memTotal,
                Used = memUsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read memory info");
            return new MemoryInfo();
        }
    }

    private async Task<DiskInfo> GetDiskInfoAsync(CancellationToken cancellationToken)
    {
        try
        {
            var mountsPath = "/host/proc/mounts";
            if (!File.Exists(mountsPath))
            {
                _logger.LogWarning("/host/proc/mounts not found, trying /proc/mounts");
                mountsPath = "/proc/mounts";
            }

            var lines = await File.ReadAllLinesAsync(mountsPath, cancellationToken);
            var rootMount = lines.FirstOrDefault(l => l.Contains(" / "));
            
            if (rootMount == null)
            {
                _logger.LogWarning("Root mount not found in {Path}", mountsPath);
                return new DiskInfo();
            }

            var driveInfo = new DriveInfo("/");
            
            return new DiskInfo
            {
                Total = driveInfo.TotalSize,
                Used = driveInfo.TotalSize - driveInfo.AvailableFreeSpace
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read disk info");
            return new DiskInfo();
        }
    }

    private async Task<LoadAverage> GetLoadAverageAsync(CancellationToken cancellationToken)
    {
        try
        {
            var loadavgPath = "/host/proc/loadavg";
            if (!File.Exists(loadavgPath))
            {
                _logger.LogWarning("/host/proc/loadavg not found, trying /proc/loadavg");
                loadavgPath = "/proc/loadavg";
            }

            var content = await File.ReadAllTextAsync(loadavgPath, cancellationToken);
            var parts = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 3)
            {
                _logger.LogWarning("Invalid loadavg format in {Path}", loadavgPath);
                return new LoadAverage();
            }

            return new LoadAverage
            {
                Load1 = decimal.Parse(parts[0], CultureInfo.InvariantCulture),
                Load5 = decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                Load15 = decimal.Parse(parts[2], CultureInfo.InvariantCulture)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read load average");
            return new LoadAverage();
        }
    }

    private record CpuStats
    {
        public long User { get; init; }
        public long Nice { get; init; }
        public long System { get; init; }
        public long Idle { get; init; }
        public long IoWait { get; init; }
        public long Irq { get; init; }
        public long SoftIrq { get; init; }
        
        public long Total => User + Nice + System + Idle + IoWait + Irq + SoftIrq;
    }

    private record MemoryInfo
    {
        public long Total { get; init; }
        public long Used { get; init; }
    }

    private record DiskInfo
    {
        public long Total { get; init; }
        public long Used { get; init; }
    }

    private record LoadAverage
    {
        public decimal Load1 { get; init; }
        public decimal Load5 { get; init; }
        public decimal Load15 { get; init; }
    }
}