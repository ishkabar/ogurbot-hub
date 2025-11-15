// File: Ogur.Hub.Domain/Entities/Device.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

using Ogur.Hub.Domain.Common;
using Ogur.Hub.Domain.Enums;
using Ogur.Hub.Domain.ValueObjects;

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a physical device registered under a license with unique fingerprint.
/// </summary>
public sealed class Device : Entity<int>
{
    /// <summary>
    /// Gets the license identifier this device belongs to.
    /// </summary>
    public int LicenseId { get; private set; }

    /// <summary>
    /// Gets the unique device fingerprint combining HWID and GUID.
    /// </summary>
    public DeviceFingerprint Fingerprint { get; private set; }

    /// <summary>
    /// Gets the friendly device name.
    /// </summary>
    public string? DeviceName { get; private set; }

    /// <summary>
    /// Gets the device description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the primary user identifier for this device.
    /// </summary>
    public int? PrimaryUserId { get; private set; }

    /// <summary>
    /// Gets the current device status.
    /// </summary>
    public DeviceStatus Status { get; private set; }

    /// <summary>
    /// Gets the last known IP address of the device.
    /// </summary>
    public string? LastIpAddress { get; private set; }

    /// <summary>
    /// Gets the last seen timestamp.
    /// </summary>
    public DateTime? LastSeenAt { get; private set; }

    /// <summary>
    /// Gets the device registration timestamp.
    /// </summary>
    public DateTime RegisteredAt { get; private set; }

    /// <summary>
    /// Gets the license this device belongs to.
    /// </summary>
    public License License { get; private set; } = null!;

    /// <summary>
    /// Gets the primary user assigned to this device.
    /// </summary>
    public User? PrimaryUser { get; private set; }

    /// <summary>
    /// Gets the collection of device sessions.
    /// </summary>
    public ICollection<DeviceSession> Sessions { get; private set; } = new List<DeviceSession>();

    /// <summary>
    /// Gets the collection of telemetry records.
    /// </summary>
    public ICollection<Telemetry> Telemetries { get; private set; } = new List<Telemetry>();

    /// <summary>
    /// Gets the collection of hub commands sent to this device.
    /// </summary>
    public ICollection<HubCommand> Commands { get; private set; } = new List<HubCommand>();

    /// <summary>
    /// Gets the collection of device user assignments for multi-user scenarios.
    /// </summary>
    public ICollection<DeviceUser> DeviceUsers { get; private set; } = new List<DeviceUser>();

    private Device()
    {
    }

    /// <summary>
    /// Creates a new device instance.
    /// </summary>
    /// <param name="licenseId">License identifier.</param>
    /// <param name="fingerprint">Device fingerprint.</param>
    /// <param name="deviceName">Device name.</param>
    /// <param name="description">Device description.</param>
    /// <param name="primaryUserId">Primary user identifier.</param>
    /// <returns>New device instance.</returns>
    public static Device Create(
        int licenseId,
        DeviceFingerprint fingerprint,
        string? deviceName = null,
        string? description = null,
        int? primaryUserId = null)
    {
        var device = new Device
        {
            LicenseId = licenseId,
            Fingerprint = fingerprint,
            DeviceName = deviceName,
            Description = description,
            PrimaryUserId = primaryUserId,
            Status = DeviceStatus.Online,
            RegisteredAt = DateTime.UtcNow
        };

        return device;
    }

    /// <summary>
    /// Updates the device status to Online.
    /// </summary>
    public void Activate()
    {
        Status = DeviceStatus.Online;
        UpdateTimestamp();
    }

    /// <summary>
    /// Updates the device status to Offline.
    /// </summary>
    public void Logout()
    {
        Status = DeviceStatus.Offline;
        UpdateTimestamp();
    }

    /// <summary>
    /// Updates the device status to Blocked.
    /// </summary>
    public void Block()
    {
        Status = DeviceStatus.Blocked;
        UpdateTimestamp();
    }
    
    /// <summary>
    /// Unblocks the device, allowing access again.
    /// </summary>
    public void Unblock()
    {
        if (Status != DeviceStatus.Blocked)
        {
            throw new InvalidOperationException("Cannot unblock a device that is not blocked");
        }

        Status = DeviceStatus.Offline;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the device is currently blocked.
    /// </summary>
    /// <returns>True if device is blocked, otherwise false.</returns>
    public bool IsBlocked()
    {
        return Status == DeviceStatus.Blocked;
    }

    /// <summary>
    /// Updates the last seen information.
    /// </summary>
    /// <param name="ipAddress">Current IP address.</param>
    public void UpdateLastSeen(string? ipAddress)
    {
        LastIpAddress = ipAddress;
        LastSeenAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    /// <summary>
    /// Updates the device name.
    /// </summary>
    /// <param name="deviceName">New device name.</param>
    public void UpdateDeviceName(string? deviceName)
    {
        DeviceName = deviceName;
        UpdateTimestamp();
    }


    /// <summary>
    /// Updates the device description.
    /// </summary>
    /// <param name="description">New description.</param>
    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdateTimestamp();
    }

    /// <summary>
    /// Sets the primary user for this device.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    public void SetPrimaryUser(int? userId)
    {
        PrimaryUserId = userId;
        UpdateTimestamp();
    }

    /// <summary>
    /// Assigns an additional user to this device.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    public void AssignUser(int userId)
    {
        if (!DeviceUsers.Any(du => du.UserId == userId))
        {
            var deviceUser = DeviceUser.Create(Id, userId);
            DeviceUsers.Add(deviceUser);
        }

        UpdateTimestamp();
    }

    /// <summary>
    /// Removes a user assignment from this device.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    public void RemoveUser(int userId)
    {
        var deviceUser = DeviceUsers.FirstOrDefault(du => du.UserId == userId);
        if (deviceUser != null)
        {
            DeviceUsers.Remove(deviceUser);
        }

        UpdateTimestamp();
    }
}