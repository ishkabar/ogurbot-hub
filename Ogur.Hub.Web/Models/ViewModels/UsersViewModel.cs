// File: Hub.Web/Models/ViewModels/UserViewModels.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

using Ogur.Hub.Web.Models.ViewModels.Base;
using Ogur.Hub.Web.Services;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for users page
/// </summary>
public sealed class UsersViewModel : BasePageViewModel
{
    /// <summary>
    /// List of users
    /// </summary>
    public List<UserDto> Users { get; set; } = new();
}

/// <summary>
/// View model for user create page
/// </summary>
public sealed class UserCreateViewModel : BasePageViewModel
{
    /// <summary>
    /// Username
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirm password
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Whether user is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether user is admin
    /// </summary>
    public bool IsAdmin { get; set; } = false;
}

/// <summary>
/// View model for user edit page
/// </summary>
public sealed class UserEditViewModel : BaseEditViewModel
{
    /// <summary>
    /// User ID to edit
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// User data for editing
    /// </summary>
    public UserDto? User { get; set; }
}

/// <summary>
/// View model for user details page
/// </summary>
public sealed class UserDetailsViewModel : BaseDetailsViewModel
{
    /// <summary>
    /// User ID to load
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// User data
    /// </summary>
    public UserDto? User { get; set; }
}