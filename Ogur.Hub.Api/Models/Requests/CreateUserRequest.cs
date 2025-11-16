// File: Ogur.Hub.Api/Models/Requests/CreateUserRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request to create a new user.
/// </summary>
/// <param name="Username">Username.</param>
/// <param name="Email">Email address.</param>
/// <param name="Password">Password.</param>
/// <param name="IsActive">Is user active.</param>
/// <param name="Role">User role.</param>
public sealed record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    bool IsActive,
    UserRole Role);