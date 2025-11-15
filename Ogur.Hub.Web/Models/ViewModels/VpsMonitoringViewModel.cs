using Ogur.Hub.Web.Models.ViewModels.Base;

namespace Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Application.DTO;


/// <summary>
/// VPS monitoring page view model
/// </summary>
public sealed class VpsMonitoringViewModel : BasePageViewModel
{
    /// <summary>
    /// Current VPS resource usage
    /// </summary>
    public VpsResourceDto? CurrentResources { get; init; }
    
    /// <summary>
    /// List of Docker containers
    /// </summary>
    public List<VpsContainerDto> Containers { get; init; } = new();
    
    /// <summary>
    /// List of monitored websites
    /// </summary>
    public List<VpsWebsiteDto> Websites { get; init; } = new();
    
    /// <summary>
    /// API base URL for SignalR connection
    /// </summary>
    public required string ApiBaseUrl { get; init; }
}