// File: Ogur.Hub.Api/Models/Responses/LicenseValidationResponse.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Responses

namespace Ogur.Hub.Api.Models.Responses;

/// <summary>
/// Response model for license validation.
/// </summary>
public sealed record LicenseValidationResponse
{
    /// <summary>
    /// Whether the license is valid.
    /// </summary>
    public required bool IsValid { get; init; }

    /// <summary>
    /// Device identifier (if registered).
    /// </summary>
    public int? DeviceId { get; init; }

    /// <summary>
    /// Whether this is a new device registration.
    /// </summary>
    public bool IsNewDevice { get; init; }

    /// <summary>
    /// License expiration date.
    /// </summary>
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// Number of devices registered.
    /// </summary>
    public int RegisteredDevices { get; init; }

    /// <summary>
    /// Maximum number of devices allowed.
    /// </summary>
    public int MaxDevices { get; init; }

    /// <summary>
    /// Error message if validation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }
}