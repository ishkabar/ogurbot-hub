// File: Hub.Web/Models/ViewModels/ApplicationViewModels.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

using Ogur.Hub.Web.Models.ViewModels.Base;
using Ogur.Hub.Web.Services;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for applications page
/// </summary>
public sealed class ApplicationsViewModel : BasePageViewModel
{
    /// <summary>
    /// List of applications
    /// </summary>
    public List<ApplicationDto> Applications { get; set; } = new();
}

/// <summary>
/// View model for application create page
/// </summary>
public sealed class ApplicationCreateViewModel : BasePageViewModel
{
    /// <summary>
    /// Application name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Application display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Application description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Current version
    /// </summary>
    public string CurrentVersion { get; set; } = "1.0.0";

    /// <summary>
    /// Whether application is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// View model for application edit page
/// </summary>
public sealed class ApplicationEditViewModel : BaseEditViewModel
{
    /// <summary>
    /// Application ID to edit
    /// </summary>
    public int ApplicationId { get; set; }
    
    /// <summary>
    /// Application data for editing
    /// </summary>
    public ApplicationDto? Application { get; set; }
}

/// <summary>
/// View model for application details page
/// </summary>
public sealed class ApplicationDetailsViewModel : BaseDetailsViewModel
{
    /// <summary>
    /// Application ID to load
    /// </summary>
    public int ApplicationId { get; set; }
    
    /// <summary>
    /// Application data
    /// </summary>
    public ApplicationDto? Application { get; set; }
    
    /// <summary>
    /// Total licenses count for this application
    /// </summary>
    public int LicensesCount { get; set; }
    
    /// <summary>
    /// Active licenses count for this application
    /// </summary>
    public int ActiveLicensesCount { get; set; }
    
    /// <summary>
    /// Total devices count for this application
    /// </summary>
    public int DevicesCount { get; set; }
    
    /// <summary>
    /// Connected devices count for this application
    /// </summary>
    public int ConnectedDevicesCount { get; set; }
}