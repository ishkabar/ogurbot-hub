// File: Ogur.Hub.Api/Models/Requests/SendCommandRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using System.ComponentModel.DataAnnotations;
using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for sending a command to a device.
/// </summary>
public sealed record SendCommandRequest
{
    /// <summary>
    /// Type of command to send.
    /// </summary>
    [Required]
    public required CommandType CommandType { get; init; }

    /// <summary>
    /// Command payload as JSON string.
    /// </summary>
    [Required]
    public required string Payload { get; init; }
}