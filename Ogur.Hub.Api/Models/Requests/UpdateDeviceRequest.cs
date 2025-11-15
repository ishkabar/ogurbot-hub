// File: Ogur.Hub.Api/Models/Requests/UpdateDeviceRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for updating device information.
/// </summary>
public sealed class UpdateDeviceRequest
{
    /// <summary>
    /// Gets or sets the device name.
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// Gets or sets the device description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the primary user identifier.
    /// </summary>
    public int? PrimaryUserId { get; set; }
}