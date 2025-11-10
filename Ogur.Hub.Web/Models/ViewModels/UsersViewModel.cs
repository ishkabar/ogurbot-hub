// File: Hub.Web/Models/ViewModels/UsersViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

using Ogur.Hub.Web.Models.ViewModels.Base;
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

/// <summary>
/// View model for user edit page
/// </summary>

public class UserEditViewModel : BaseEditViewModel
{
    /// <summary>
    /// User ID to edit
    /// </summary>
    public int UserId { get; set; }
}

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

public class UserCreateViewModel : BasePageViewModel
{
    
}