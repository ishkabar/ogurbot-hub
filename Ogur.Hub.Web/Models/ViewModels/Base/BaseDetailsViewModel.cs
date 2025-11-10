// File: Hub.Web/Models/ViewModels/Base/BaseDetailsViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels.Base

namespace Ogur.Hub.Web.Models.ViewModels.Base;

/// <summary>
/// Base view model for details pages
/// </summary>
public abstract class BaseDetailsViewModel : BasePageViewModel
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
    /// Edit action name
    /// </summary>
    public string EditAction { get; set; } = "Edit";
}