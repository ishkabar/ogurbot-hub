// File: Ogur.Hub.Infrastructure/Services/FakeVpsDataGenerator.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Services

using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Infrastructure.Services;

/// <summary>
/// Generates fake VPS monitoring data for development/testing.
/// </summary>
public static class FakeVpsDataGenerator
{
    private static readonly Random _random = new();
    private static readonly string[] _containerNames = { "ogurhub_api", "ogurhub_web", "traefik", "mariadb", "redis" };
    private static readonly string[] _images = { "ogur/hub-api:latest", "ogur/hub-web:latest", "traefik:v2.10", "mariadb:10.11", "redis:7-alpine" };
    private static readonly string[] _domains = { "hub.ogur.dev", "api.hub.ogur.dev", "ogur.dev", "dkarczewski.com" };

    /// <summary>
    /// Generates fake container data.
    /// </summary>
    public static List<VpsContainerDto> GenerateContainers(int count = 5)
    {
        var containers = new List<VpsContainerDto>();
        
        for (int i = 0; i < count; i++)
        {
            containers.Add(new VpsContainerDto
            {
                Id = i + 1,
                ContainerId = Guid.NewGuid().ToString("N")[..12],
                Name = _containerNames[i % _containerNames.Length],
                Image = _images[i % _images.Length],
                State = "running",
                Status = "Up " + _random.Next(1, 30) + " days",
                CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 90)),
                StartedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 30)),
                CpuUsagePercent = (decimal)(_random.NextDouble() * 50),
                MemoryUsageMb = (decimal)(_random.Next(100, 2000)),
                MemoryLimitMb = 2048,
                MemoryUsagePercent = (decimal)(_random.NextDouble() * 80),
                NetworkRxMb = (decimal)(_random.NextDouble() * 1000),
                NetworkTxMb = (decimal)(_random.NextDouble() * 500),
                LastUpdatedAt = DateTime.UtcNow
            });
        }

        return containers;
    }

    /// <summary>
    /// Generates fake website data.
    /// </summary>
    public static List<VpsWebsiteDto> GenerateWebsites(int count = 4)
    {
        var websites = new List<VpsWebsiteDto>();
        
        for (int i = 0; i < count; i++)
        {
            var isActive = _random.Next(0, 10) > 1;
            
            websites.Add(new VpsWebsiteDto
            {
                Id = i + 1,
                Domain = _domains[i % _domains.Length],
                ServiceName = _containerNames[i % _containerNames.Length],
                ContainerName = _containerNames[i % _containerNames.Length],
                SslEnabled = true,
                SslExpiresAt = DateTime.UtcNow.AddDays(_random.Next(30, 90)),
                IsActive = isActive,
                LastCheckedAt = DateTime.UtcNow.AddSeconds(-_random.Next(0, 300)),
                LastStatusCode = isActive ? 200 : 503,
                LastResponseTimeMs = isActive ? _random.Next(50, 500) : null,
                SslExpiringSoon = _random.Next(0, 10) > 8
            });
        }

        return websites;
    }

    /// <summary>
    /// Generates fake resource data.
    /// </summary>
    public static VpsResourceDto GenerateResources()
    {
        var memTotal = 16m;
        var memUsed = (decimal)(_random.NextDouble() * 12);
        var diskTotal = 100m;
        var diskUsed = (decimal)(_random.NextDouble() * 60);

        return new VpsResourceDto
        {
            CpuUsagePercent = (decimal)(_random.NextDouble() * 60),
            MemoryTotalGb = memTotal,
            MemoryUsedGb = memUsed,
            MemoryUsagePercent = (memUsed / memTotal) * 100,
            DiskTotalGb = diskTotal,
            DiskUsedGb = diskUsed,
            DiskUsagePercent = (diskUsed / diskTotal) * 100,
            NetworkRxMbps = (decimal)(_random.NextDouble() * 100),
            NetworkTxMbps = (decimal)(_random.NextDouble() * 50),
            LoadAverage1Min = (decimal)(_random.NextDouble() * 4),
            LoadAverage5Min = (decimal)(_random.NextDouble() * 3),
            LoadAverage15Min = (decimal)(_random.NextDouble() * 2),
            Timestamp = DateTime.UtcNow
        };
    }
}