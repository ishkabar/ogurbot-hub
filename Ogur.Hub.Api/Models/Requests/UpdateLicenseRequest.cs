// File: Hub.Api/Models/Requests/UpdateLicenseRequest.cs
// Project: Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request to update a license
/// </summary>
public sealed record UpdateLicenseRequest
{
    /// <summary>
    /// Maximum devices allowed
    /// </summary>
    public int MaxDevices { get; init; }

    /// <summary>
    /// License expiration date
    /// </summary>
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// License status
    /// </summary>
    public int Status { get; init; }
    
    /// <summary>
    /// License description
    /// </summary>
    public string? Description { get; init; }
}