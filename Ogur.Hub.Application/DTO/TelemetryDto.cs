// File: Ogur.Hub.Application/DTO/TelemetryDto.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.DTO

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// DTO representing a telemetry entry.
/// </summary>
/// <param name="Id">Telemetry ID.</param>
/// <param name="DeviceId">Device ID.</param>
/// <param name="EventType">Event type.</param>
/// <param name="EventDataJson">Event data as JSON.</param>
/// <param name="OccurredAt">When event occurred.</param>
/// <param name="ReceivedAt">When hub received event.</param>
public sealed record TelemetryDto(
    int Id,
    int DeviceId,
    string EventType,
    string? EventDataJson,
    DateTime OccurredAt,
    DateTime ReceivedAt);