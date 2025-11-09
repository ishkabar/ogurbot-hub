// File: Ogur.Hub.Api/Models/Requests/TelemetryBatchRequest.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Requests

using System.ComponentModel.DataAnnotations;

namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for submitting a batch of telemetry events.
/// </summary>
public sealed record TelemetryBatchRequest
{
    /// <summary>
    /// Collection of telemetry events.
    /// </summary>
    [Required]
    public required List<TelemetryEventRequest> Events { get; init; }
}

/// <summary>
/// Single telemetry event.
/// </summary>
public sealed record TelemetryEventRequest
{
    /// <summary>
    /// Type of the event.
    /// </summary>
    [Required]
    public required string EventType { get; init; }

    /// <summary>
    /// Event data as JSON string.
    /// </summary>
    public string? EventDataJson { get; init; }

    /// <summary>
    /// When the event occurred.
    /// </summary>
    [Required]
    public required DateTime OccurredAt { get; init; }
}