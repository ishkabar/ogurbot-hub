// File: Ogur.Hub.Application/Common/Interfaces/ICommandDispatcher.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Common.Interfaces

using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Application.Common.Interfaces;

/// <summary>
/// Interface for dispatching commands to connected devices via SignalR.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches a command to a device.
    /// </summary>
    /// <param name="command">Command to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if command was dispatched, false if device not connected.</returns>
    Task<bool> DispatchCommandAsync(HubCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to a device.
    /// </summary>
    /// <param name="deviceId">Target device ID.</param>
    /// <param name="message">Notification message.</param>
    /// <param name="severity">Severity level.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if notification was sent, false if device not connected.</returns>
    Task<bool> SendNotificationAsync(int deviceId, string message, string severity, CancellationToken cancellationToken = default);
}