// File: Ogur.Hub.Domain/Entities/License.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

using Ogur.Hub.Domain.Common;
using Ogur.Hub.Domain.ValueObjects;
using Ogur.Hub.Domain.Common;
using Ogur.Hub.Domain.ValueObjects;
using Ogur.Hub.Domain.Enums;


namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a software license that grants access to an application for a specific user.
/// </summary>
public sealed class License : AggregateRoot<int>
{
    /// <summary>
    /// Gets the application identifier this license is for.
    /// </summary>
    public int ApplicationId { get; private set; }

    /// <summary>
    /// Gets the user identifier who owns this license.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Gets the unique license key.
    /// </summary>
    public LicenseKey LicenseKey { get; private set; }

    /// <summary>
    /// Gets the maximum number of devices allowed for this license.
    /// </summary>
    public int MaxDevices { get; private set; }

    /// <summary>
    /// Gets the license start date.
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// Gets the license expiration date.
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>
    /// Gets whether the license is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the current status of the license.
    /// </summary>
    public LicenseStatus Status { get; private set; } = LicenseStatus.Active;

    /// <summary>
    /// Gets the application this license is for.
    /// </summary>
    public Application Application { get; private set; } = null!;

    /// <summary>
    /// Gets the user who owns this license.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of devices registered under this license.
    /// </summary>
    public ICollection<Device> Devices { get; private set; } = new List<Device>();

    private License()
    {
    }

    /// <summary>
    /// Creates a new license instance.
    /// </summary>
    /// <param name="applicationId">Application identifier.</param>
    /// <param name="userId">User identifier.</param>
    /// <param name="maxDevices">Maximum allowed devices.</param>
    /// <param name="startDate">License start date.</param>
    /// <param name="endDate">License end date.</param>
    /// <returns>New license instance.</returns>
    public static License Create(
        int applicationId,
        int userId,
        int maxDevices = 2,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var license = new License
        {
            ApplicationId = applicationId,
            UserId = userId,
            LicenseKey = LicenseKey.Generate(),
            MaxDevices = maxDevices,
            StartDate = startDate ?? DateTime.UtcNow,
            EndDate = endDate,
            IsActive = true,
            Status = LicenseStatus.Active
        };

        return license;
    }

    /// <summary>
    /// Validates if the license is currently valid.
    /// </summary>
    /// <returns>True if license is valid, otherwise false.</returns>
    public bool IsValid()
    {
        if (!IsActive)
            return false;

        var now = DateTime.UtcNow;
        if (now < StartDate)
            return false;

        if (EndDate.HasValue && now > EndDate.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if a new device can be registered under this license.
    /// </summary>
    /// <returns>True if device slot is available, otherwise false.</returns>
    public bool CanRegisterDevice()
    {
        return Devices.Count < MaxDevices;
    }

    /// <summary>
    /// Activates the license.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        Status = LicenseStatus.Active;
        UpdateTimestamp();
    }

    /// <summary>
    /// Deactivates the license.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        Status = LicenseStatus.Revoked;
        UpdateTimestamp();
    }
    
    /// <summary>
    /// Extends the license expiration date.
    /// </summary>
    /// <param name="newEndDate">New expiration date (null for no expiration).</param>
    public void ExtendLicense(DateTime? newEndDate)
    {
        if (newEndDate.HasValue && newEndDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("End date must be in the future", nameof(newEndDate));

        EndDate = newEndDate;
        UpdateTimestamp();
    }
    
    
    /// <summary>
    /// Updates license properties
    /// </summary>
    /// <param name="maxDevices">Maximum devices</param>
    /// <param name="endDate">Expiration date</param>
    /// <param name="status">License status</param>
    public void Update(int maxDevices, DateTime? endDate, LicenseStatus status)
    {
        MaxDevices = maxDevices;
        EndDate = endDate;
        Status = status;

        // Update IsActive based on status
        IsActive = status == LicenseStatus.Active;
        UpdateTimestamp();
    }
}