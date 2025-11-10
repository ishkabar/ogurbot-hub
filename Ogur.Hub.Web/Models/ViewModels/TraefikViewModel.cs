// File: Hub.Web/Models/ViewModels/TraefikViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for Traefik dashboard page
/// </summary>
public class TraefikViewModel : BasePageViewModel
{
    /// <summary>
    /// Traefik dashboard URL (with embedded credentials if available)
    /// </summary>
    public string DashboardUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates whether credentials are configured
    /// </summary>
    public bool HasCredentials { get; set; }
}