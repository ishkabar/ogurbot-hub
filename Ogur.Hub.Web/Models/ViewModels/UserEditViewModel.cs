// File: Hub.Web/Models/ViewModels/UserEditViewModel.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Models.ViewModels

namespace Ogur.Hub.Web.Models.ViewModels;

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