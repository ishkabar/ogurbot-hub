// File: Ogur.Hub.Infrastructure/SignalR/DevicesHub.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.SignalR

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Ogur.Hub.Infrastructure.SignalR;

/// <summary>
/// SignalR hub for real-time communication with connected devices.
/// </summary>
public sealed class DevicesHub : Hub<IDevicesHubClient>
{
    private readonly IRepository<Device, int> _deviceRepository;
    private readonly IRepository<DeviceSession, int> _sessionRepository;
    private readonly IRepository<HubCommand, int> _commandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DevicesHub> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesHub"/> class.
    /// </summary>
    /// <param name="deviceRepository">Device repository.</param>
    /// <param name="sessionRepository">Device session repository.</param>
    /// <param name="commandRepository">Hub command repository.</param>
    /// <param name="unitOfWork">Unit of work for database transactions.</param>
    /// <param name="logger">Logger instance.</param>
    public DevicesHub(
        IRepository<Device, int> deviceRepository,
        IRepository<DeviceSession, int> sessionRepository,
        IRepository<HubCommand, int> commandRepository,
        IUnitOfWork unitOfWork,
        ILogger<DevicesHub> logger)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _commandRepository = commandRepository ?? throw new ArgumentNullException(nameof(commandRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Called when a device connects to the hub.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task OnConnectedAsync()
    {
        var deviceId = GetDeviceIdFromContext();
        var ipAddress = GetIpAddressFromContext();
        var userAgent = GetUserAgentFromContext();

        if (deviceId == null)
        {
            _logger.LogWarning("Connection attempt without valid device ID from {IpAddress}", ipAddress);
            Context.Abort();
            return;
        }

        var device = await _deviceRepository.GetByIdAsync(deviceId.Value);
        if (device == null)
        {
            _logger.LogWarning("Connection attempt with unknown device ID {DeviceId} from {IpAddress}", 
                deviceId, ipAddress);
            Context.Abort();
            return;
        }

        if (device.IsBlocked())
        {
            _logger.LogWarning("Blocked device {DeviceId} attempted to connect from {IpAddress}", 
                deviceId, ipAddress);
            Context.Abort();
            return;
        }

        var session = DeviceSession.Create(
            deviceId: device.Id,
            connectionId: Context.ConnectionId,
            ipAddress: ipAddress,
            userAgent: userAgent);

        await _sessionRepository.AddAsync(session);

        device.UpdateLastSeen(ipAddress);
        device.Activate();
        _deviceRepository.Update(device);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Device {DeviceId} connected with session {SessionId} from {IpAddress}", 
            deviceId, session.Id, ipAddress);

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a device disconnects from the hub.
    /// </summary>
    /// <param name="exception">Exception that caused the disconnection, if any.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var session = await _sessionRepository.FirstOrDefaultAsync(
            s => s.ConnectionId == Context.ConnectionId && s.DisconnectedAt == null);

        if (session != null)
        {
            session.Disconnect();
            _sessionRepository.Update(session);

            var device = await _deviceRepository.GetByIdAsync(session.DeviceId);
            if (device != null)
            {
                device.Logout();
                _deviceRepository.Update(device);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Device session {SessionId} disconnected", session.Id);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Handles heartbeat from connected devices to maintain session alive status.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Heartbeat()
    {
        var session = await _sessionRepository.FirstOrDefaultAsync(
            s => s.ConnectionId == Context.ConnectionId && s.DisconnectedAt == null);

        if (session != null)
        {
            session.UpdateHeartbeat();
            _sessionRepository.Update(session);
            await _unitOfWork.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Handles command acknowledgement from devices.
    /// </summary>
    /// <param name="clientGuid">Client GUID of the acknowledged command.</param>
    /// <param name="success">Whether the command executed successfully.</param>
    /// <param name="errorMessage">Error message if command failed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AcknowledgeCommand(Guid clientGuid, bool success, string? errorMessage = null)
    {
        var command = await _commandRepository.FirstOrDefaultAsync(c => c.ClientGuid == clientGuid);
        if (command == null)
        {
            _logger.LogWarning("Received acknowledgement for unknown command {ClientGuid}", clientGuid);
            return;
        }

        if (success)
        {
            command.MarkCompleted();
        }
        else
        {
            command.MarkFailed(errorMessage ?? "Unknown error");
        }

        _commandRepository.Update(command);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Command {CommandId} (ClientGuid: {ClientGuid}) acknowledged with status {Status}", 
            command.Id, clientGuid, command.Status);
    }


    private int? GetDeviceIdFromContext()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            return null;
        }

        var deviceIdStr = httpContext.Request.Query["deviceId"].FirstOrDefault();
        if (string.IsNullOrEmpty(deviceIdStr))
        {
            return null;
        }

        return int.TryParse(deviceIdStr, out var deviceId) ? deviceId : null;
    }

    private string? GetIpAddressFromContext()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            return null;
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserAgentFromContext()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            return null;
        }

        return httpContext.Request.Headers.UserAgent.FirstOrDefault();
    }
}