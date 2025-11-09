// File: Ogur.Hub.Api/Models/Requests/ValidateLicenseRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using System.ComponentModel.DataAnnotations;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for license validation.
/// </summary>
public sealed record ValidateLicenseRequest
{
    /// <summary>
    /// License key to validate.
    /// </summary>
    [Required]
    public required string LicenseKey { get; init; }

    /// <summary>
    /// Hardware identifier of the device.
    /// </summary>
    [Required]
    public required string Hwid { get; init; }

    /// <summary>
    /// Unique device GUID.
    /// </summary>
    [Required]
    public required Guid DeviceGuid { get; init; }

    /// <summary>
    /// Optional device name.
    /// </summary>
    public string? DeviceName { get; init; }
}