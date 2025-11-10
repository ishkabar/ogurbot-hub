using Ogur.Hub.Web.Models.ViewModels.Base;

namespace Ogur.Hub.Web.Models.ViewModels;


/// <summary>
/// VPS monitoring page view model
/// </summary>
public sealed class VpsMonitoringViewModel : BasePageViewModel
{
    /// <summary>
    /// Current VPS resource usage
    /// </summary>
    public Services.VpsResourceDto? CurrentResources { get; init; }
    
    /// <summary>
    /// List of Docker containers
    /// </summary>
    public List<Services.VpsContainerDto> Containers { get; init; } = new();
    
    /// <summary>
    /// List of monitored websites
    /// </summary>
    public List<Services.VpsWebsiteDto> Websites { get; init; } = new();
    
    /// <summary>
    /// API base URL for SignalR connection
    /// </summary>
    public required string ApiBaseUrl { get; init; }
}