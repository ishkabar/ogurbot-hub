// File: Ogur.Hub.Api/Models/Responses/LoginResponse.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Responses

namespace Ogur.Hub.Api.Models.Responses;

/// <summary>
/// Response model for successful login.
/// </summary>
public sealed record LoginResponse
{
    /// <summary>
    /// JWT access token.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Token type (always "Bearer").
    /// </summary>
    public string TokenType { get; init; } = "Bearer";

    /// <summary>
    /// Token expiration time in seconds.
    /// </summary>
    public required int ExpiresIn { get; init; }

    /// <summary>
    /// User identifier.
    /// </summary>
    public required int UserId { get; init; }

    /// <summary>
    /// Username.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Whether the user is an administrator.
    /// </summary>
    public required bool IsAdmin { get; init; }
}