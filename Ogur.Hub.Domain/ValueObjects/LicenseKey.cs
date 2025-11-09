// File: Ogur.Hub.Domain/ValueObjects/LicenseKey.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.ValueObjects

using Ogur.Hub.Domain.Common;

namespace Ogur.Hub.Domain.ValueObjects;

/// <summary>
/// Value object representing a license key with validation.
/// </summary>
public sealed class LicenseKey : ValueObject
{
    /// <summary>
    /// Gets the license key value.
    /// </summary>
    public string Value { get; private set; }

    private LicenseKey(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new license key from a string value.
    /// </summary>
    /// <param name="value">The license key string.</param>
    /// <returns>A new LicenseKey instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the key format is invalid.</exception>
    public static LicenseKey Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("License key cannot be empty", nameof(value));

        if (!IsValidFormat(value))
            throw new ArgumentException("Invalid license key format", nameof(value));

        return new LicenseKey(value.ToUpperInvariant());
    }

    /// <summary>
    /// Generates a new random license key.
    /// Format: OGUR-XXXX-XXXX-XXXX-XXXX
    /// </summary>
    /// <param name="prefix">Optional prefix (default: OGUR).</param>
    /// <returns>A new randomly generated LicenseKey.</returns>
    public static LicenseKey Generate(string prefix = "OGUR")
    {
        var segments = new List<string> { prefix };
        
        for (int i = 0; i < 4; i++)
        {
            segments.Add(GenerateRandomSegment(4));
        }

        return new LicenseKey(string.Join("-", segments));
    }

    private static bool IsValidFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var parts = value.Split('-');
        return parts.Length == 5 && parts.All(p => p.Length >= 3 && p.Length <= 8);
    }

    private static string GenerateRandomSegment(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }

    /// <summary>
    /// Gets the atomic values that define equality.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the license key.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicit conversion to string.
    /// </summary>
    public static implicit operator string(LicenseKey key) => key.Value;
}