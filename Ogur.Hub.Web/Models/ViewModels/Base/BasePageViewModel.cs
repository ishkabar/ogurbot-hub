// File: Hub.Web/Models/ViewModels/Base/BasePageViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels.Base

using Ogur.Hub.Web.Services;

namespace Ogur.Hub.Web.Models.ViewModels.Base;

/// <summary>
/// Base view model for all pages providing common properties
/// </summary>
public abstract class BasePageViewModel
{
    /// <summary>
    /// Page title
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// Page description
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Current username from session
    /// </summary>
    public string? Username { get; init; }
    
    /// <summary>
    /// Whether user is admin
    /// </summary>
    public bool IsAdmin { get; init; }
}












