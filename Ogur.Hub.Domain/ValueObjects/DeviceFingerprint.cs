// File: Ogur.Hub.Domain/ValueObjects/DeviceFingerprint.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.ValueObjects

using Ogur.Hub.Domain.Common;

namespace Ogur.Hub.Domain.ValueObjects;

/// <summary>
/// Value object representing a unique device fingerprint (HWID + GUID).
/// </summary>
public sealed class DeviceFingerprint : ValueObject
{
    /// <summary>
    /// Gets the hardware ID (HWID).
    /// </summary>
    public string Hwid { get; private set; }

    /// <summary>
    /// Gets the unique device GUID.
    /// </summary>
    public Guid DeviceGuid { get; private set; }

    private DeviceFingerprint(string hwid, Guid deviceGuid)
    {
        Hwid = hwid;
        DeviceGuid = deviceGuid;
    }

    /// <summary>
    /// Creates a new device fingerprint.
    /// </summary>
    /// <param name="hwid">Hardware ID.</param>
    /// <param name="deviceGuid">Device GUID.</param>
    /// <returns>A new DeviceFingerprint instance.</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
    public static DeviceFingerprint Create(string hwid, Guid deviceGuid)
    {
        if (string.IsNullOrWhiteSpace(hwid))
            throw new ArgumentException("HWID cannot be empty", nameof(hwid));

        if (deviceGuid == Guid.Empty)
            throw new ArgumentException("Device GUID cannot be empty", nameof(deviceGuid));

        return new DeviceFingerprint(hwid, deviceGuid);
    }

    /// <summary>
    /// Gets the atomic values that define equality.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Hwid;
        yield return DeviceGuid;
    }

    /// <summary>
    /// Returns a string representation combining HWID and GUID.
    /// </summary>
    public override string ToString() => $"{Hwid}:{DeviceGuid}";
}