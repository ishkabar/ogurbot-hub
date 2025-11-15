// File: Hub.Domain/Entities/DeviceUser.cs
// Project: Hub.Domain
// Namespace: Hub.Domain.Entities

using Ogur.Hub.Domain.Common;

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a many-to-many relationship between devices and users for multi-user device scenarios.
/// </summary>
public sealed class DeviceUser : Entity<int>
{
    /// <summary>
    /// Gets the device identifier.
    /// </summary>
    public int DeviceId { get; private set; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Gets when this user was assigned to the device.
    /// </summary>
    public DateTime AssignedAt { get; private set; }

    /// <summary>
    /// Gets the device.
    /// </summary>
    public Device Device { get; private set; } = null!;

    /// <summary>
    /// Gets the user.
    /// </summary>
    public User User { get; private set; } = null!;

    private DeviceUser() { }

    /// <summary>
    /// Creates a new device-user assignment.
    /// </summary>
    /// <param name="deviceId">Device identifier.</param>
    /// <param name="userId">User identifier.</param>
    /// <returns>New device-user instance.</returns>
    public static DeviceUser Create(int deviceId, int userId)
    {
        return new DeviceUser
        {
            DeviceId = deviceId,
            UserId = userId,
            AssignedAt = DateTime.UtcNow
        };
    }
}