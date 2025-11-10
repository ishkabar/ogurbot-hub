// File: Hub.Web/Models/ViewModels/DevicesViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

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