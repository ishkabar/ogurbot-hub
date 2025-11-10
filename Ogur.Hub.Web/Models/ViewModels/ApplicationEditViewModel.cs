// File: Hub.Web/Models/ViewModels/ApplicationEditViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for application edit page
/// </summary>
public sealed class ApplicationEditViewModel : BaseDetailsViewModel
{
    /// <summary>
    /// Application data
    /// </summary>
    public ApplicationViewDto Application { get; set; } = null!;
}