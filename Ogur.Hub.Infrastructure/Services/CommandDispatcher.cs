// File: Ogur.Hub.Infrastructure/Services/CommandDispatcher.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Services

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using Ogur.Hub.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Ogur.Hub.Infrastructure.Services;

/// <summary>
/// Service responsible for dispatching commands to connected devices via SignalR.
/// </summary>
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IHubContext<DevicesHub, IDevicesHubClient> _hubContext;
    private readonly IRepository<DeviceSession, int> _sessionRepository;
    private readonly ILogger<CommandDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
    /// </summary>
    /// <param name="hubContext">SignalR hub context.</param>
    /// <param name="sessionRepository">Device session repository.</param>
    /// <param name="logger">Logger instance.</param>
    public CommandDispatcher(
        IHubContext<DevicesHub, IDevicesHubClient> hubContext,
        IRepository<DeviceSession, int> sessionRepository,
        ILogger<CommandDispatcher> logger)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> DispatchCommandAsync(
        HubCommand command, 
        CancellationToken cancellationToken = default)
    {
        var activeSessions = await _sessionRepository.FindAsync(
            s => s.DeviceId == command.DeviceId && s.DisconnectedAt == null,
            cancellationToken);

        if (!activeSessions.Any())
        {
            _logger.LogWarning("No active sessions found for device {DeviceId}", command.DeviceId);
            return false;
        }

        var dispatchTasks = activeSessions.Select(session =>
            DispatchToConnectionAsync(session.ConnectionId, command, cancellationToken));

        var results = await Task.WhenAll(dispatchTasks);

        return results.Any(success => success);
    }

    /// <inheritdoc />
    public async Task<bool> SendNotificationAsync(
        int deviceId, 
        string message, 
        string severity, 
        CancellationToken cancellationToken = default)
    {
        var activeSessions = await _sessionRepository.FindAsync(
            s => s.DeviceId == deviceId && s.DisconnectedAt == null,
            cancellationToken);

        if (!activeSessions.Any())
        {
            _logger.LogWarning("No active sessions found for device {DeviceId}", deviceId);
            return false;
        }

        var notificationTasks = activeSessions.Select(session =>
            _hubContext.Clients.Client(session.ConnectionId)
                .ReceiveNotification(message, severity));

        await Task.WhenAll(notificationTasks);

        _logger.LogInformation("Notification sent to device {DeviceId} via {Count} sessions", 
            deviceId, activeSessions.Count);

        return true;
    }

    private async Task<bool> DispatchToConnectionAsync(
        string connectionId, 
        HubCommand command, 
        CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients.Client(connectionId)
                .ReceiveCommand(command.ClientGuid, command.CommandType.ToString(), command.Payload);

            _logger.LogInformation("Command {CommandId} (ClientGuid: {ClientGuid}) dispatched to connection {ConnectionId}", 
                command.Id, command.ClientGuid, connectionId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch command {CommandId} to connection {ConnectionId}", 
                command.Id, connectionId);
            return false;
        }
    }
}