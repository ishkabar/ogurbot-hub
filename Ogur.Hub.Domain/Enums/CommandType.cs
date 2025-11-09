// File: Ogur.Hub.Domain/Enums/CommandType.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Enums

namespace Ogur.Hub.Domain.Enums;

/// <summary>
/// Types of commands that can be sent to devices via SignalR.
/// </summary>
public enum CommandType
{
    /// <summary>
    /// Force logout from the application.
    /// </summary>
    Logout = 1,

    /// <summary>
    /// Block the device from connecting.
    /// </summary>
    BlockDevice = 2,

    /// <summary>
    /// Send a notification to the device.
    /// </summary>
    Notify = 3,

    /// <summary>
    /// Force the application to update.
    /// </summary>
    ForceUpdate = 4,

    /// <summary>
    /// Refresh license information.
    /// </summary>
    RefreshLicense = 5,

    /// <summary>
    /// Custom command with payload.
    /// </summary>
    Custom = 99
}