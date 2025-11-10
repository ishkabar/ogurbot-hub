// File: Hub.Web/Models/ViewModels/DeviceDetailsViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

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