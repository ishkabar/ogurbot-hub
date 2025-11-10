// File: Hub.Web/Models/ViewModels/BaseEditViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// Base view model for edit pages
/// </summary>
public abstract class BaseEditViewModel : BasePageViewModel
{
    /// <summary>
    /// Entity ID
    /// </summary>
    public int EntityId { get; set; }
    
    /// <summary>
    /// Controller name for navigation
    /// </summary>
    public string ControllerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Details action name
    /// </summary>
    public string DetailsAction { get; set; } = "Details";
}