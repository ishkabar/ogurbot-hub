// File: Hub.Api/Models/Requests/ExtendLicenseRequest.cs
// Project: Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request to extend license expiration
/// </summary>
public sealed record ExtendLicenseRequest
{
    /// <summary>
    /// New expiration date (null for no expiration)
    /// </summary>
    public DateTime? NewExpirationDate { get; init; }
}