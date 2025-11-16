// File: Ogur.Hub.Api/Models/Responses/RegisterResponse.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Responses

namespace Ogur.Hub.Api.Models.Responses;

/// <summary>
/// Response model for user registration.
/// </summary>
public sealed class RegisterResponse
{
    /// <summary>
    /// Gets or sets the registered user ID.
    /// </summary>
    public required int UserId { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the registration success message.
    /// </summary>
    public required string Message { get; set; }
}