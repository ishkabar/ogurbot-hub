// File: Hub.Web/Models/ViewModels/UsersViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

using Ogur.Hub.Web.Services;

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for users list page
/// </summary>
public class UsersViewModel : BasePageViewModel
{
    /// <summary>
    /// List of users
    /// </summary>
    public List<UserDto> Users { get; set; } = new();
}