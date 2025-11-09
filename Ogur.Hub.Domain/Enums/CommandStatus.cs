// File: Ogur.Hub.Domain/Enums/CommandStatus.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Enums

namespace Ogur.Hub.Domain.Enums;

/// <summary>
/// Status of a command sent to a device.
/// </summary>
public enum CommandStatus
{
    /// <summary>
    /// Command is pending delivery.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Command has been sent to the device.
    /// </summary>
    Sent = 2,

    /// <summary>
    /// Device acknowledged receiving the command.
    /// </summary>
    Acknowledged = 3,

    /// <summary>
    /// Command executed successfully.
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Command execution failed.
    /// </summary>
    Failed = 5,

    /// <summary>
    /// Command timed out.
    /// </summary>
    TimedOut = 6
}