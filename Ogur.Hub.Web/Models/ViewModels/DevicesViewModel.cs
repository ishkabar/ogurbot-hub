// File: Hub.Web/Models/ViewModels/DevicesViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

using Ogur.Hub.Web.Models.ViewModels.Base;

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for devices page
/// </summary>
public sealed class DevicesViewModel : BasePageViewModel
{
    /// <summary>
    /// List of devices
    /// </summary>
    public List<Services.DeviceDto> Devices { get; set; } = new();

    /// <summary>
    /// Optional license ID filter
    /// </summary>
    public int? LicenseId { get; set; }
}

/// <summary>
/// View model for device edit page
/// </summary>
public class DeviceEditViewModel : BaseEditViewModel
{
    /// <summary>
    /// Device ID to edit
    /// </summary>
    public int DeviceId { get; set; }
}

/// <summary>
/// View model for device details page
/// </summary>
public class DeviceDetailsViewModel : BaseDetailsViewModel
{
    /// <summary>
    /// Device ID to load
    /// </summary>
    public int DeviceId { get; set; }
}