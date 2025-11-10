// File: Hub.Web/Models/ViewModels/LicenseEditViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for license edit page
/// </summary>
public class LicenseEditViewModel : BaseEditViewModel
{
    /// <summary>
    /// License ID to edit
    /// </summary>
    public int LicenseId { get; set; }
}