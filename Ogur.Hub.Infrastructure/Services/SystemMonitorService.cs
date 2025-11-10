// File: Ogur.Hub.Infrastructure/Services/SystemMonitorService.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Services

using System.Diagnostics;
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
            var snapshot = new VpsResourceSnapshot
            {
                Timestamp = DateTime.UtcNow,
                CpuUsagePercent = await GetCpuUsageAsync(cancellationToken),
                MemoryTotalBytes = await GetTotalMemoryAsync(cancellationToken),
                MemoryUsedBytes = await GetUsedMemoryAsync(cancellationToken),
                DiskTotalBytes = await GetTotalDiskAsync(cancellationToken),
                DiskUsedBytes = await GetUsedDiskAsync(cancellationToken),
                NetworkRxBytesPerSec = 0,
                NetworkTxBytesPerSec = 0,
                LoadAverage1Min = await GetLoadAverage1Async(cancellationToken),
                LoadAverage5Min = await GetLoadAverage5Async(cancellationToken),
                LoadAverage15Min = await GetLoadAverage15Async(cancellationToken)
            };

            await _vpsRepository.AddSnapshotAsync(snapshot, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to capture resource snapshot");
            throw;
        }
    }

    private async Task<decimal> GetCpuUsageAsync(CancellationToken cancellationToken)
    {
        var output = await ExecuteCommandAsync("top -bn1 | grep 'Cpu(s)' | awk '{print $2}'", cancellationToken);
        return decimal.TryParse(output.Replace("%", ""), out var result) ? result : 0;
    }

    private async Task<long> GetTotalMemoryAsync(CancellationToken cancellationToken)
    {
        var output = await ExecuteCommandAsync("free -b | grep Mem | awk '{print $2}'", cancellationToken);
        return long.TryParse(output, out var result) ? result : 0;
    }

    private async Task<long> GetUsedMemoryAsync(CancellationToken cancellationToken)
    {
        var output = await ExecuteCommandAsync("free -b | grep Mem | awk '{print $3}'", cancellationToken);
        return long.TryParse(output, out var result) ? result : 0;
    }

    private async Task<long> GetTotalDiskAsync(CancellationToken cancellationToken)
    {
        var output = await ExecuteCommandAsync("df -B1 / | tail -1 | awk '{print $2}'", cancellationToken);
        return long.TryParse(output, out var result) ? result : 0;
    }

    private async Task<long> GetUsedDiskAsync(CancellationToken cancellationToken)
    {
        var output = await ExecuteCommandAsync("df -B1 / | tail -1 | awk '{print $3}'", cancellationToken);
        return long.TryParse(output, out var result) ? result : 0;
    }

    private async Task<decimal> GetLoadAverage1Async(CancellationToken cancellationToken)
    {
        var output = await ExecuteCommandAsync("uptime | awk -F'load average:' '{print $2}' | awk '{print $1}'", cancellationToken);
        return decimal.TryParse(output.Replace(",", ""), out var result) ? result : 0;
    }

    private async Task<decimal> GetLoadAverage5Async(CancellationToken cancellationToken)
    {
        var output = await ExecuteCommandAsync("uptime | awk -F'load average:' '{print $2}' | awk '{print $2}'", cancellationToken);
        return decimal.TryParse(output.Replace(",", ""), out var result) ? result : 0;
    }

    private async Task<decimal> GetLoadAverage15Async(CancellationToken cancellationToken)
    {
        var output = await ExecuteCommandAsync("uptime | awk -F'load average:' '{print $2}' | awk '{print $3}'", cancellationToken);
        return decimal.TryParse(output.Replace(",", ""), out var result) ? result : 0;
    }

    private static async Task<string> ExecuteCommandAsync(string command, CancellationToken cancellationToken)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        return output.Trim();
    }
}