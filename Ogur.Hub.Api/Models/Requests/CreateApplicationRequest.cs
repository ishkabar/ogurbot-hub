// File: Ogur.Hub.Api/Models/Requests/CreateApplicationRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using System.ComponentModel.DataAnnotations;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for creating a new application.
/// </summary>
public sealed record CreateApplicationRequest
{
    /// <summary>
    /// Unique application name (identifier).
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; init; }

    /// <summary>
    /// Display name for the application.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public required string DisplayName { get; init; }

    /// <summary>
    /// Current version of the application.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string CurrentVersion { get; init; }

    /// <summary>
    /// Optional description of the application.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; init; }
}