// File: Hub.Web/Models/ViewModels/ApplicationDetailsViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for application details page
/// </summary>
public sealed class ApplicationDetailsViewModel : BaseDetailsViewModel
{
    /// <summary>
    /// Application data
    /// </summary>
    public ApplicationViewDto Application { get; set; } = null!;
    
    /// <summary>
    /// Total licenses count
    /// </summary>
    public int LicensesCount { get; set; }
    
    /// <summary>
    /// Active licenses count
    /// </summary>
    public int ActiveLicensesCount { get; set; }
    
    /// <summary>
    /// Total devices count
    /// </summary>
    public int DevicesCount { get; set; }
    
    /// <summary>
    /// Connected devices count
    /// </summary>
    public int ConnectedDevicesCount { get; set; }
}

/// <summary>
/// Application data for view
/// </summary>
public sealed class ApplicationViewDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CurrentVersion { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ApiKey { get; set; }
}