// File: Hub.Web/Models/ViewModels/DeviceEditViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

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