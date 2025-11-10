// File: Hub.Web/Models/ViewModels/LicenseDetailsViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for license details page
/// </summary>
public class LicenseDetailsViewModel : BaseDetailsViewModel
{
    /// <summary>
    /// License ID to load
    /// </summary>
    public int LicenseId { get; set; }
}