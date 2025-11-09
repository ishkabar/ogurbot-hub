// File: Ogur.Hub.Domain/Entities/DeviceSession.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

using Ogur.Hub.Domain.Common;

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents an active SignalR connection session for a device.
/// </summary>
public sealed class DeviceSession : Entity<int>
{
    /// <summary>
    /// Gets the device identifier this session belongs to.
    /// </summary>
    public int DeviceId { get; private set; }

    /// <summary>
    /// Gets the SignalR connection identifier.
    /// </summary>
    public string ConnectionId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the IP address of the connection.
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// Gets the user agent string.
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// Gets the connection start timestamp.
    /// </summary>
    public DateTime ConnectedAt { get; private set; }

    /// <summary>
    /// Gets the connection end timestamp.
    /// </summary>
    public DateTime? DisconnectedAt { get; private set; }

    /// <summary>
    /// Gets the last heartbeat timestamp.
    /// </summary>
    public DateTime? LastHeartbeatAt { get; private set; }

    /// <summary>
    /// Gets the device this session belongs to.
    /// </summary>
    public Device Device { get; private set; } = null!;

    private DeviceSession() { }

    /// <summary>
    /// Creates a new device session.
    /// </summary>
    /// <param name="deviceId">Device identifier.</param>
    /// <param name="connectionId">SignalR connection ID.</param>
    /// <param name="ipAddress">IP address.</param>
    /// <param name="userAgent">User agent string.</param>
    /// <returns>New device session instance.</returns>
    public static DeviceSession Create(int deviceId, string connectionId, string? ipAddress, string? userAgent)
    {
        var session = new DeviceSession
        {
            DeviceId = deviceId,
            ConnectionId = connectionId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ConnectedAt = DateTime.UtcNow,
            LastHeartbeatAt = DateTime.UtcNow
        };
        
        return session;
    }

    /// <summary>
    /// Marks the session as disconnected.
    /// </summary>
    public void Disconnect()
    {
        DisconnectedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    /// <summary>
    /// Updates the last heartbeat timestamp.
    /// </summary>
    public void UpdateHeartbeat()
    {
        LastHeartbeatAt = DateTime.UtcNow;
        UpdateTimestamp();
    }
}