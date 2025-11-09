// File: Ogur.Hub.Application/DTO/DeviceDto.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.DTO

using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// DTO representing a device.
/// </summary>
/// <param name="Id">Device ID.</param>
/// <param name="LicenseId">License ID.</param>
/// <param name="Hwid">Hardware ID.</param>
/// <param name="DeviceGuid">Device GUID.</param>
/// <param name="DeviceName">Device name.</param>
/// <param name="Status">Device status.</param>
/// <param name="LastIpAddress">Last known IP address.</param>
/// <param name="RegisteredAt">Registration timestamp.</param>
/// <param name="LastSeenAt">Last seen timestamp.</param>
/// <param name="BlockedAt">Block timestamp.</param>
/// <param name="BlockReason">Block reason.</param>
/// <param name="ConnectionId">Current SignalR connection ID.</param>
/// <param name="ConnectedAt">Connection timestamp.</param>
public sealed record DeviceDto(
    int Id,
    int LicenseId,
    string Hwid,
    string DeviceGuid,
    string DeviceName,
    DeviceStatus Status,
    string? LastIpAddress,
    DateTime RegisteredAt,
    DateTime LastSeenAt,
    DateTime? BlockedAt,
    string? BlockReason,
    string? ConnectionId,
    DateTime? ConnectedAt);