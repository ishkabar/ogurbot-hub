// File: Ogur.Hub.Infrastructure/Services/DockerMonitorService.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Services

using System.Diagnostics;
using System.Text.Json;
using Ogur.Hub.Application.Interfaces;
using Ogur.Hub.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Ogur.Hub.Infrastructure.Services;

/// <summary>
/// Service for monitoring Docker containers via Docker CLI.
/// </summary>
public class DockerMonitorService : IDockerMonitorService
{
    private readonly IVpsRepository _vpsRepository;
    private readonly ILogger<DockerMonitorService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerMonitorService"/> class.
    /// </summary>
    /// <param name="vpsRepository">VPS repository.</param>
    /// <param name="logger">Logger instance.</param>
    public DockerMonitorService(IVpsRepository vpsRepository, ILogger<DockerMonitorService> logger)
    {
        _vpsRepository = vpsRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task UpdateContainersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var containers = await GetDockerContainersAsync(cancellationToken);
            var now = DateTime.UtcNow;

            foreach (var dockerContainer in containers)
            {
                var existingContainer =
                    await _vpsRepository.GetContainerByDockerIdAsync(dockerContainer.Id, cancellationToken);

                if (existingContainer != null)
                {
                    existingContainer.Name = dockerContainer.Name;
                    existingContainer.Image = dockerContainer.Image;
                    existingContainer.State = dockerContainer.State;
                    existingContainer.Status = dockerContainer.Status;
                    existingContainer.StartedAt = dockerContainer.StartedAt;
                    existingContainer.LastUpdatedAt = now;

                    await _vpsRepository.UpdateContainerAsync(existingContainer, cancellationToken);
                }
                else
                {
                    var newContainer = new VpsContainer
                    {
                        ContainerId = dockerContainer.Id,
                        Name = dockerContainer.Name,
                        Image = dockerContainer.Image,
                        State = dockerContainer.State,
                        Status = dockerContainer.Status,
                        CreatedAt = dockerContainer.CreatedAt,
                        StartedAt = dockerContainer.StartedAt,
                        LastUpdatedAt = now
                    };

                    await _vpsRepository.AddContainerAsync(newContainer, cancellationToken);
                }
            }

            var currentContainerIds = containers.Select(c => c.Id).ToHashSet();
            var oldContainers = await _vpsRepository.GetAllContainersAsync(cancellationToken);

            var containersToDelete = oldContainers.Where(c => !currentContainerIds.Contains(c.ContainerId)).ToList();
            foreach (var old in containersToDelete)
            {
                await _vpsRepository.DeleteContainerAsync(old, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update Docker containers");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateContainerStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var containers = await _vpsRepository.GetAllContainersAsync(cancellationToken);
            var now = DateTime.UtcNow;

            foreach (var container in containers)
            {
                if (container.State != "running") continue;

                var stats = await GetContainerStatsAsync(container.ContainerId, cancellationToken);
                if (stats == null) continue;

                container.CpuUsagePercent = stats.CpuPercent;
                container.MemoryUsageBytes = stats.MemoryUsage;
                container.MemoryLimitBytes = stats.MemoryLimit;
                container.NetworkRxBytes = stats.NetworkRx;
                container.NetworkTxBytes = stats.NetworkTx;
                container.LastUpdatedAt = now;

                await _vpsRepository.UpdateContainerAsync(container, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update container stats");
            throw;
        }
    }

    private async Task<List<DockerContainerInfo>> GetDockerContainersAsync(CancellationToken cancellationToken)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "ps -a --format json",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        var containers = new List<DockerContainerInfo>();
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var container = JsonSerializer.Deserialize<DockerContainerJson>(line);
            if (container == null) continue;

            containers.Add(new DockerContainerInfo
            {
                Id = container.ID,
                Name = container.Names,
                Image = container.Image,
                State = container.State,
                Status = container.Status,
                CreatedAt = ParseDockerTimestamp(container.CreatedAt),
                StartedAt = container.State == "running" ? DateTime.UtcNow : null
            });
        }

        return containers;
    }

    private async Task<DockerStats?> GetContainerStatsAsync(string containerId, CancellationToken cancellationToken)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"stats {containerId} --no-stream --format json",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(output)) return null;

        var statsJson = JsonSerializer.Deserialize<DockerStatsJson>(output);
        if (statsJson == null) return null;

        return new DockerStats
        {
            CpuPercent = ParsePercentage(statsJson.CPUPerc),
            MemoryUsage = ParseMemory(statsJson.MemUsage.Split('/')[0]),
            MemoryLimit = ParseMemory(statsJson.MemUsage.Split('/')[1]),
            NetworkRx = ParseMemory(statsJson.NetIO.Split('/')[0]),
            NetworkTx = ParseMemory(statsJson.NetIO.Split('/')[1])
        };
    }

    private static DateTime ParseDockerTimestamp(string timestamp)
    {
        if (DateTime.TryParse(timestamp, out var result))
            return result.ToUniversalTime();
        return DateTime.UtcNow;
    }

    private static decimal ParsePercentage(string value)
    {
        var cleaned = value.Replace("%", "").Trim();
        return decimal.TryParse(cleaned, out var result) ? result : 0;
    }

    private static long ParseMemory(string value)
    {
        var cleaned = value.Trim();
        var multiplier = 1L;

        if (cleaned.EndsWith("KiB")) multiplier = 1024L;
        else if (cleaned.EndsWith("MiB")) multiplier = 1024L * 1024L;
        else if (cleaned.EndsWith("GiB")) multiplier = 1024L * 1024L * 1024L;

        cleaned = cleaned.Replace("KiB", "").Replace("MiB", "").Replace("GiB", "").Trim();
        return decimal.TryParse(cleaned, out var result) ? (long)(result * multiplier) : 0;
    }

    private record DockerContainerInfo
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Image { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? StartedAt { get; init; }
    }

    private record DockerStats
    {
        public decimal CpuPercent { get; init; }
        public long MemoryUsage { get; init; }
        public long MemoryLimit { get; init; }
        public long NetworkRx { get; init; }
        public long NetworkTx { get; init; }
    }

    private record DockerContainerJson
    {
        public string ID { get; init; } = string.Empty;
        public string Names { get; init; } = string.Empty;
        public string Image { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string CreatedAt { get; init; } = string.Empty;
    }

    private record DockerStatsJson
    {
        public string CPUPerc { get; init; } = string.Empty;
        public string MemUsage { get; init; } = string.Empty;
        public string NetIO { get; init; } = string.Empty;
    }
}