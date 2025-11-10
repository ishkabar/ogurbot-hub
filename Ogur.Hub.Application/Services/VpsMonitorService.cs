// File: Ogur.Hub.Application/Services/VpsMonitorService.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Services

using Ogur.Hub.Application.DTO;
using Ogur.Hub.Application.Interfaces;
using Ogur.Hub.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Ogur.Hub.Application.Services;

/// <summary>
/// Implementation of VPS monitoring service.
/// </summary>
public class VpsMonitorService : IVpsMonitorService
{
    private readonly IVpsRepository _vpsRepository;
    private readonly IDockerMonitorService _dockerMonitor;
    private readonly ISystemMonitorService _systemMonitor;
    private readonly ILogger<VpsMonitorService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="VpsMonitorService"/> class.
    /// </summary>
    /// <param name="vpsRepository">VPS repository.</param>
    /// <param name="dockerMonitor">Docker monitoring service.</param>
    /// <param name="systemMonitor">System monitoring service.</param>
    /// <param name="logger">Logger instance.</param>
    public VpsMonitorService(
        IVpsRepository vpsRepository,
        IDockerMonitorService dockerMonitor,
        ISystemMonitorService systemMonitor,
        ILogger<VpsMonitorService> logger)
    {
        _vpsRepository = vpsRepository;
        _dockerMonitor = dockerMonitor;
        _systemMonitor = systemMonitor;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<List<VpsContainerDto>> GetContainersAsync(CancellationToken cancellationToken = default)
    {
        var containers = await _vpsRepository.GetAllContainersAsync(cancellationToken);

        return containers
            .OrderBy(c => c.Name)
            .Select(c => new VpsContainerDto
            {
                Id = c.Id,
                ContainerId = c.ContainerId,
                Name = c.Name,
                Image = c.Image,
                State = c.State,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                StartedAt = c.StartedAt,
                CpuUsagePercent = c.CpuUsagePercent,
                MemoryUsageMb = c.MemoryUsageBytes / 1024m / 1024m,
                MemoryLimitMb = c.MemoryLimitBytes / 1024m / 1024m,
                MemoryUsagePercent = c.MemoryLimitBytes > 0 ? c.MemoryUsageBytes * 100m / c.MemoryLimitBytes : 0,
                NetworkRxMb = c.NetworkRxBytes / 1024m / 1024m,
                NetworkTxMb = c.NetworkTxBytes / 1024m / 1024m,
                LastUpdatedAt = c.LastUpdatedAt
            })
            .ToList();
    }

    /// <inheritdoc/>
    public async Task<List<VpsWebsiteDto>> GetWebsitesAsync(CancellationToken cancellationToken = default)
    {
        var websites = await _vpsRepository.GetAllWebsitesAsync(cancellationToken);

        return websites
            .OrderBy(w => w.Domain)
            .Select(w => new VpsWebsiteDto
            {
                Id = w.Id,
                Domain = w.Domain,
                ServiceName = w.ServiceName,
                ContainerName = w.Container?.Name,
                SslEnabled = w.SslEnabled,
                SslExpiresAt = w.SslExpiresAt,
                IsActive = w.IsActive,
                LastCheckedAt = w.LastCheckedAt,
                LastStatusCode = w.LastStatusCode,
                LastResponseTimeMs = w.LastResponseTimeMs,
                SslExpiringSoon = w.SslExpiresAt.HasValue && w.SslExpiresAt.Value < DateTime.UtcNow.AddDays(30)
            })
            .ToList();
    }

    /// <inheritdoc/>
    public async Task<VpsResourceDto> GetCurrentResourcesAsync(CancellationToken cancellationToken = default)
    {
        var snapshots = await _vpsRepository.GetAllSnapshotsAsync(cancellationToken);
        var latest = snapshots.OrderByDescending(s => s.Timestamp).FirstOrDefault();

        if (latest == null)
        {
            return new VpsResourceDto
            {
                Timestamp = DateTime.UtcNow
            };
        }

        return MapToDto(latest);
    }

    /// <inheritdoc/>
    public async Task<List<VpsResourceDto>> GetResourceHistoryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var snapshots = await _vpsRepository.GetAllSnapshotsAsync(cancellationToken);
        
        return snapshots
            .Where(s => s.Timestamp >= from && s.Timestamp <= to)
            .OrderBy(s => s.Timestamp)
            .Select(MapToDto)
            .ToList();
    }

    /// <inheritdoc/>
    public async Task RefreshContainerStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dockerMonitor.UpdateContainersAsync(cancellationToken);
            await _dockerMonitor.UpdateContainerStatsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh container stats");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task RefreshWebsiteHealthAsync(CancellationToken cancellationToken = default)
    {
        var websites = await _vpsRepository.GetAllWebsitesAsync(cancellationToken);
        var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        foreach (var website in websites)
        {
            try
            {
                var protocol = website.SslEnabled ? "https" : "http";
                var url = $"{protocol}://{website.Domain}";
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var response = await httpClient.GetAsync(url, cancellationToken);
                stopwatch.Stop();

                website.LastStatusCode = (int)response.StatusCode;
                website.LastResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                website.IsActive = response.IsSuccessStatusCode;
                website.LastCheckedAt = DateTime.UtcNow;

                await _vpsRepository.UpdateWebsiteAsync(website, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Health check failed for domain: {Domain}", website.Domain);
                website.LastStatusCode = 0;
                website.LastResponseTimeMs = null;
                website.IsActive = false;
                website.LastCheckedAt = DateTime.UtcNow;

                await _vpsRepository.UpdateWebsiteAsync(website, cancellationToken);
            }
        }
    }

    /// <inheritdoc/>
    public async Task CaptureResourceSnapshotAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _systemMonitor.CaptureSnapshotAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to capture resource snapshot");
            throw;
        }
    }

    private static VpsResourceDto MapToDto(VpsResourceSnapshot snapshot)
    {
        return new VpsResourceDto
        {
            CpuUsagePercent = snapshot.CpuUsagePercent,
            MemoryTotalGb = snapshot.MemoryTotalBytes / 1024m / 1024m / 1024m,
            MemoryUsedGb = snapshot.MemoryUsedBytes / 1024m / 1024m / 1024m,
            MemoryUsagePercent = snapshot.MemoryTotalBytes > 0 ? snapshot.MemoryUsedBytes * 100m / snapshot.MemoryTotalBytes : 0,
            DiskTotalGb = snapshot.DiskTotalBytes / 1024m / 1024m / 1024m,
            DiskUsedGb = snapshot.DiskUsedBytes / 1024m / 1024m / 1024m,
            DiskUsagePercent = snapshot.DiskTotalBytes > 0 ? snapshot.DiskUsedBytes * 100m / snapshot.DiskTotalBytes : 0,
            NetworkRxMbps = snapshot.NetworkRxBytesPerSec / 1024m / 1024m,
            NetworkTxMbps = snapshot.NetworkTxBytesPerSec / 1024m / 1024m,
            LoadAverage1Min = snapshot.LoadAverage1Min,
            LoadAverage5Min = snapshot.LoadAverage5Min,
            LoadAverage15Min = snapshot.LoadAverage15Min,
            Timestamp = snapshot.Timestamp
        };
    }
}