// File: Ogur.Hub.Domain/Entities/Telemetry.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

using Ogur.Hub.Domain.Common;

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents telemetry data received from an application.
/// </summary>
public sealed class Telemetry : Entity<int>
{
    /// <summary>
    /// Gets the device ID that sent this telemetry.
    /// </summary>
    public int DeviceId { get; private set; }

    /// <summary>
    /// Gets the event type (e.g., "cpu_usage", "error", "user_action").
    /// </summary>
    public string EventType { get; private set; }

    /// <summary>
    /// Gets the event data as JSON string.
    /// </summary>
    public string? EventDataJson { get; private set; }

    /// <summary>
    /// Gets when the event occurred on the client side.
    /// </summary>
    public DateTime OccurredAt { get; private set; }

    /// <summary>
    /// Gets when the hub received the telemetry.
    /// </summary>
    public DateTime ReceivedAt { get; private set; }

    /// <summary>
    /// Gets the navigation property to the device.
    /// </summary>
    public Device Device { get; private set; } = null!;

    private Telemetry() { }

    private Telemetry(int deviceId, string eventType, string? eventDataJson, DateTime occurredAt)
    {
        DeviceId = deviceId;
        EventType = eventType;
        EventDataJson = eventDataJson;
        OccurredAt = occurredAt;
        ReceivedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new telemetry entry.
    /// </summary>
    /// <param name="deviceId">Device ID.</param>
    /// <param name="eventType">Event type.</param>
    /// <param name="eventDataJson">Event data as JSON.</param>
    /// <param name="occurredAt">When the event occurred.</param>
    /// <returns>A new Telemetry instance.</returns>
    public static Telemetry Create(int deviceId, string eventType, string? eventDataJson, DateTime occurredAt)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type is required", nameof(eventType));

        return new Telemetry(deviceId, eventType, eventDataJson, occurredAt);
    }
}