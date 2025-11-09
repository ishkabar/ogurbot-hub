// File: Ogur.Hub.Api/Models/Requests/UpdateLicenseRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using System.ComponentModel.DataAnnotations;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for updating an existing license.
/// </summary>
public sealed record UpdateLicenseRequest
{
    /// <summary>
    /// Maximum number of devices allowed.
    /// </summary>
    [Range(1, 100)]
    public int? MaxDevices { get; init; }

    /// <summary>
    /// License expiration date.
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Whether the license is active.
    /// </summary>
    public bool? IsActive { get; init; }
}