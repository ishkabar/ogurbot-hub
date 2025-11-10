// File: Hub.Application/DTO/UserDto.cs
// Project: Hub.Application
// Namespace: Ogur.Hub.Application.DTO

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// User data transfer object.
/// </summary>
/// <param name="Id">User ID.</param>
/// <param name="Username">Username.</param>
/// <param name="Email">Email address.</param>
/// <param name="IsActive">Is user active.</param>
/// <param name="IsAdmin">Is user admin.</param>
/// <param name="LicensesCount">Number of licenses.</param>
/// <param name="CreatedAt">Created timestamp.</param>
/// <param name="LastLoginAt">Last login timestamp.</param>
public sealed record UserDto(
    int Id,
    string Username,
    string Email,
    bool IsActive,
    bool IsAdmin,
    int LicensesCount,
    DateTime CreatedAt,
    DateTime? LastLoginAt);