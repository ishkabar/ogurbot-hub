namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// Dashboard page view model
/// </summary>
public sealed class DashboardViewModel : BasePageViewModel
{
    /// <summary>
    /// Dashboard statistics
    /// </summary>
    public DashboardStatsDto Stats { get; init; } = new();
}

/// <summary>
/// Dashboard statistics data transfer object
/// </summary>
public sealed class DashboardStatsDto
{
    /// <summary>
    /// Total number of applications
    /// </summary>
    public int TotalApplications { get; init; }
    
    /// <summary>
    /// Number of active licenses
    /// </summary>
    public int ActiveLicenses { get; init; }
    
    /// <summary>
    /// Number of currently connected devices
    /// </summary>
    public int ConnectedDevices { get; init; }
    
    /// <summary>
    /// Number of commands sent today
    /// </summary>
    public int CommandsToday { get; init; }
}