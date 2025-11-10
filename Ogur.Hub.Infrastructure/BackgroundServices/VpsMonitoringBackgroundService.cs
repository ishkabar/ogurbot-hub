// File: Ogur.Hub.Infrastructure/BackgroundServices/VpsMonitoringBackgroundService.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.BackgroundServices

using Ogur.Hub.Application.Interfaces;
using Ogur.Hub.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Hub.Infrastructure.Services;


namespace Ogur.Hub.Infrastructure.BackgroundServices;

/// <summary>
/// Background service for VPS monitoring tasks with real-time updates.
/// </summary>
public class VpsMonitoringBackgroundService : BackgroundService
{
    private readonly ILogger<VpsMonitoringBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<VpsHub> _hubContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="VpsMonitoringBackgroundService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="serviceProvider">Service provider for scoped services.</param>
    /// <param name="hubContext">SignalR hub context for real-time updates.</param>
    public VpsMonitoringBackgroundService(
        ILogger<VpsMonitoringBackgroundService> logger,
        IServiceProvider serviceProvider,
        IHubContext<VpsHub> hubContext)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
    }

    // File: Ogur.Hub.Infrastructure/BackgroundServices/VpsMonitoringBackgroundService.cs
// Zmień ExecuteAsync:

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    
    if (!OperatingSystem.IsLinux() && !isDevelopment)
    {
        _logger.LogInformation("VPS Monitoring is only available on Linux. Service will not start on {OS}", Environment.OSVersion);
        return;
    }

    if (isDevelopment && !OperatingSystem.IsLinux())
    {
        _logger.LogInformation("VPS Monitoring running in DEVELOPMENT mode with fake data");
        await RunDevelopmentModeAsync(stoppingToken);
        return;
    }

    _logger.LogInformation("VPS Monitoring Background Service started with real-time updates");

    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var vpsMonitorService = scope.ServiceProvider.GetRequiredService<IVpsMonitorService>();

            await vpsMonitorService.RefreshContainerStatsAsync(stoppingToken);
            await vpsMonitorService.CaptureResourceSnapshotAsync(stoppingToken);
            await vpsMonitorService.RefreshWebsiteHealthAsync(stoppingToken);

            var containers = await vpsMonitorService.GetContainersAsync(stoppingToken);
            var websites = await vpsMonitorService.GetWebsitesAsync(stoppingToken);
            var resources = await vpsMonitorService.GetCurrentResourcesAsync(stoppingToken);

            await _hubContext.Clients.Group("VpsMonitoring").SendAsync(
                "VpsStatsUpdated",
                new
                {
                    containers,
                    websites,
                    resources,
                    timestamp = DateTime.UtcNow
                },
                stoppingToken);

            _logger.LogDebug("Pushed VPS stats to connected clients");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in VPS monitoring background service");
        }

        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
    }
}

/// <summary>
/// Runs development mode with fake data generation.
/// </summary>
private async Task RunDevelopmentModeAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            var containers = FakeVpsDataGenerator.GenerateContainers();
            var websites = FakeVpsDataGenerator.GenerateWebsites();
            var resources = FakeVpsDataGenerator.GenerateResources();

            await _hubContext.Clients.Group("VpsMonitoring").SendAsync(
                "VpsStatsUpdated",
                new
                {
                    containers,
                    websites,
                    resources,
                    timestamp = DateTime.UtcNow
                },
                stoppingToken);

            _logger.LogDebug("Pushed FAKE VPS stats to connected clients (Development Mode)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in VPS monitoring development mode");
        }

        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Co 5 sekund w dev dla szybszego testowania
    }
}
}