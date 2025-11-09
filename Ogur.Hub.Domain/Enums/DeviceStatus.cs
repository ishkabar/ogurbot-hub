// File: Ogur.Hub.Domain/Enums/DeviceStatus.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Enums

namespace Ogur.Hub.Domain.Enums;

/// <summary>
/// Represents the connection status of a device.
/// </summary>
public enum DeviceStatus
{
    /// <summary>
    /// Device is currently connected and active.
    /// </summary>
    Online = 1,

    /// <summary>
    /// Device is disconnected but license is valid.
    /// </summary>
    Offline = 2,

    /// <summary>
    /// Device has been blocked by administrator.
    /// </summary>
    Blocked = 3,

    /// <summary>
    /// Device has performance or health issues.
    /// </summary>
    Warning = 4
}