// File: Ogur.Hub.Api/Models/Requests/RegisterRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using System.ComponentModel.DataAnnotations;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for user registration.
/// </summary>
public sealed class RegisterRequest
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username can only contain letters, numbers, dots, underscores and hyphens")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address (optional).
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }
}