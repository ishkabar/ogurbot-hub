// File: Hub.Infrastructure/Persistence/Converters/LicenseKeyConverter.cs
// Project: Hub.Infrastructure
// Namespace: Hub.Infrastructure.Persistence.Converters

using Ogur.Hub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ogur.Hub.Infrastructure.Persistence.Converters;

/// <summary>
/// EF Core value converter for LicenseKey value object.
/// </summary>
public sealed class LicenseKeyConverter : ValueConverter<LicenseKey, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseKeyConverter"/> class.
    /// </summary>
    public LicenseKeyConverter()
        : base(
            licenseKey => licenseKey.Value,
            value => LicenseKey.Create(value))
    {
    }
}