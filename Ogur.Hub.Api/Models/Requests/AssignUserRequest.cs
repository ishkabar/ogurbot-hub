// File: Ogur.Hub.Api/Models/Requests/AssignUserRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for assigning a user to a device.
/// </summary>
public sealed class AssignUserRequest
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public int UserId { get; set; }
}