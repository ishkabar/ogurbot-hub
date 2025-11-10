// File: Ogur.Hub.Infrastructure/SignalR/VpsHub.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.SignalR

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Ogur.Hub.Infrastructure.SignalR;

/// <summary>
/// SignalR hub for real-time VPS monitoring updates.
/// </summary>
public class VpsHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly ILogger<VpsHub> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="VpsHub"/> class.
    /// </summary>
    public VpsHub(ILogger<VpsHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("VPS monitoring client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("VPS monitoring client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Joins the VPS monitoring group.
    /// </summary>
    public async Task JoinMonitoring()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "VpsMonitoring");
        _logger.LogInformation("Client {ConnectionId} joined VPS monitoring", Context.ConnectionId);
    }

    /// <summary>
    /// Leaves the VPS monitoring group.
    /// </summary>
    public async Task LeaveMonitoring()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "VpsMonitoring");
        _logger.LogInformation("Client {ConnectionId} left VPS monitoring", Context.ConnectionId);
    }
}