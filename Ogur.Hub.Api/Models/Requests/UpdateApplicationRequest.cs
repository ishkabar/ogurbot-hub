// File: Hub.Api/Models/Requests/UpdateApplicationRequest.cs
// Project: Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request for updating an application.
/// </summary>
public sealed record UpdateApplicationRequest
{
    /// <summary>
    /// Application name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Display name for the application.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Optional description of the application.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Current version of the application.
    /// </summary>
    public required string CurrentVersion { get; init; }

    /// <summary>
    /// Whether the application is active.
    /// </summary>
    public required bool IsActive { get; init; }
}