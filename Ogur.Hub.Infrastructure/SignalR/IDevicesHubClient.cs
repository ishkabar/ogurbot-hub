// File: Ogur.Hub.Infrastructure/SignalR/IDevicesHubClient.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.SignalR

namespace Ogur.Hub.Infrastructure.SignalR;

/// <summary>
/// Strongly-typed interface for client methods that can be invoked from the DevicesHub.
/// </summary>
public interface IDevicesHubClient
{
    /// <summary>
    /// Sends a command to the connected device client.
    /// </summary>
    /// <param name="commandId">Unique command identifier.</param>
    /// <param name="commandType">Type of command to execute.</param>
    /// <param name="payload">Command payload as JSON string.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ReceiveCommand(Guid commandId, string commandType, string payload);

    /// <summary>
    /// Sends a notification to the connected device client.
    /// </summary>
    /// <param name="message">Notification message.</param>
    /// <param name="severity">Notification severity level.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ReceiveNotification(string message, string severity);
}