// File: Ogur.Hub.Infrastructure/Persistence/Converters/DeviceFingerprintConverter.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence.Converters

using Ogur.Hub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ogur.Hub.Infrastructure.Persistence.Converters;

/// <summary>
/// EF Core value converter for DeviceFingerprint value object.
/// </summary>
public sealed class DeviceFingerprintConverter : ValueConverter<DeviceFingerprint, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceFingerprintConverter"/> class.
    /// </summary>
    public DeviceFingerprintConverter()
        : base(
            fingerprint => fingerprint.ToString(),
            value => ParseFingerprint(value))
    {
    }

    private static DeviceFingerprint ParseFingerprint(string value)
    {
        var parts = value.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid fingerprint format", nameof(value));

        var hwid = parts[0];
        var deviceGuid = Guid.Parse(parts[1]);

        return DeviceFingerprint.Create(hwid, deviceGuid);
    }
}