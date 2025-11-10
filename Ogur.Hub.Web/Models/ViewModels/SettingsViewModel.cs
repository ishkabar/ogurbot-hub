// File: Hub.Web/Models/ViewModels/SettingsViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

using Ogur.Hub.Web.Models.ViewModels.Base;

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for settings page
/// </summary>
public sealed class SettingsViewModel : BasePageViewModel
{
    /// <summary>
    /// Current theme preference
    /// </summary>
    public string Theme { get; set; } = "light";
    
    /// <summary>
    /// Language preference
    /// </summary>
    public string Language { get; set; } = "en";
}