// File: Hub.Web/Models/ViewModels/UserDetailsViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for user details page
/// </summary>
public class UserDetailsViewModel : BaseDetailsViewModel
{
    /// <summary>
    /// User ID to load
    /// </summary>
    public int UserId { get; set; }
}