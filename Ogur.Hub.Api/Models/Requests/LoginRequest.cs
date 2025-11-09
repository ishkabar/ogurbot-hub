// File: Ogur.Hub.Api/Models/Requests/LoginRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using System.ComponentModel.DataAnnotations;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for user login.
/// </summary>
public sealed record LoginRequest
{
    /// <summary>
    /// Username or email.
    /// </summary>
    [Required]
    public required string Username { get; init; }

    /// <summary>
    /// User password.
    /// </summary>
    [Required]
    public required string Password { get; init; }
}