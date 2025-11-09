// File: Ogur.Hub.Application/DTO/ApplicationDto.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.DTO

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// DTO representing an application.
/// </summary>
/// <param name="Id">Application ID.</param>
/// <param name="Name">Application name.</param>
/// <param name="DisplayName">Display name.</param>
/// <param name="Description">Description.</param>
/// <param name="CurrentVersion">Current version.</param>
/// <param name="IsActive">Whether application is active.</param>
/// <param name="CreatedAt">Creation timestamp.</param>
/// <param name="UpdatedAt">Last update timestamp.</param>
public sealed record ApplicationDto(
    int Id,
    string Name,
    string DisplayName,
    string? Description,
    string CurrentVersion,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);