// File: Ogur.Hub.Api/Services/ITokenService.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Services

namespace Ogur.Hub.Api.Services;

/// <summary>
/// Service for generating JWT tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="username">Username.</param>
    /// <param name="isAdmin">Whether the user is an administrator.</param>
    /// <returns>JWT token string.</returns>
    string GenerateToken(int userId, string username, bool isAdmin);
}