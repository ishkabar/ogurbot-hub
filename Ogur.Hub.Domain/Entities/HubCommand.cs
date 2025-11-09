// File: Ogur.Hub.Domain/Entities/HubCommand.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

using Ogur.Hub.Domain.Common;
using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a command sent from hub to a specific device.
/// </summary>
public sealed class HubCommand : Entity<int>
{
    /// <summary>
    /// Gets the target device identifier.
    /// </summary>
    public int DeviceId { get; private set; }

    /// <summary>
    /// Gets the command type.
    /// </summary>
    public CommandType CommandType { get; private set; }

    /// <summary>
    /// Gets the command payload as JSON string.
    /// </summary>
    public string Payload { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the command status.
    /// </summary>
    public CommandStatus Status { get; private set; }

    /// <summary>
    /// Gets the scheduled send timestamp.
    /// </summary>
    public DateTime SentAt { get; private set; }

    /// <summary>
    /// Gets the acknowledgement timestamp.
    /// </summary>
    public DateTime? AcknowledgedAt { get; private set; }

    /// <summary>
    /// Gets the error message if command failed.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Gets the GUID used for client communication.
    /// </summary>
    public Guid ClientGuid { get; private set; }

    /// <summary>
    /// Gets the device this command targets.
    /// </summary>
    public Device Device { get; private set; } = null!;

    private HubCommand() { }

    /// <summary>
    /// Creates a new hub command.
    /// </summary>
    /// <param name="deviceId">Target device ID.</param>
    /// <param name="commandType">Command type.</param>
    /// <param name="payload">Command payload JSON.</param>
    /// <param name="sendAt">Optional scheduled send time.</param>
    /// <returns>New hub command instance.</returns>
    public static HubCommand Create(int deviceId, CommandType commandType, string payload, DateTime? sendAt = null)
    {
        var command = new HubCommand
        {
            DeviceId = deviceId,
            CommandType = commandType,
            Payload = payload,
            Status = CommandStatus.Pending,
            SentAt = sendAt ?? DateTime.UtcNow,
            ClientGuid = Guid.NewGuid()
        };
        
        return command;
    }

    /// <summary>
    /// Marks the command as sent.
    /// </summary>
    public void MarkSent()
    {
        Status = CommandStatus.Sent;
        UpdateTimestamp();
    }

    /// <summary>
    /// Marks the command as acknowledged by device.
    /// </summary>
    public void MarkAcknowledged()
    {
        Status = CommandStatus.Acknowledged;
        AcknowledgedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    /// <summary>
    /// Marks the command as completed successfully.
    /// </summary>
    public void MarkCompleted()
    {
        Status = CommandStatus.Completed;
        UpdateTimestamp();
    }

    /// <summary>
    /// Marks the command as failed with error message.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    public void MarkFailed(string errorMessage)
    {
        Status = CommandStatus.Failed;
        ErrorMessage = errorMessage;
        UpdateTimestamp();
    }

    /// <summary>
    /// Marks the command as timed out.
    /// </summary>
    public void MarkTimedOut()
    {
        Status = CommandStatus.TimedOut;
        ErrorMessage = "Command timed out";
        UpdateTimestamp();
    }
}