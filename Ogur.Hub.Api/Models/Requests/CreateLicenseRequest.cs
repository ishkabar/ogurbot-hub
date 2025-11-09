// File: Ogur.Hub.Api/Models/Requests/CreateLicenseRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using System.ComponentModel.DataAnnotations;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for creating a new license.
/// </summary>
public sealed record CreateLicenseRequest
{
    /// <summary>
    /// Application identifier.
    /// </summary>
    [Required]
    public required int ApplicationId { get; init; }

    /// <summary>
    /// User identifier.
    /// </summary>
    [Required]
    public required int UserId { get; init; }

    /// <summary>
    /// Maximum number of devices allowed.
    /// </summary>
    [Range(1, 100)]
    public int MaxDevices { get; init; } = 2;

    /// <summary>
    /// License start date.
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// License expiration date.
    /// </summary>
    public DateTime? EndDate { get; init; }
}