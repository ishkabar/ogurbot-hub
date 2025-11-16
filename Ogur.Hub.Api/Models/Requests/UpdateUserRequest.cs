// File: Ogur.Hub.Api/Models/Requests/UpdateUserRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request to update user details.
/// </summary>
/// <param name="Email">Email address.</param>
/// <param name="IsActive">Is user active.</param>
/// <param name="Role">User role.</param>
public sealed record UpdateUserRequest(
    string Email,
    bool IsActive,
    UserRole Role);