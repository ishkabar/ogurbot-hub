// File: Ogur.Hub.Domain/ValueObjects/ApiKey.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.ValueObjects

using System.Security.Cryptography;
using System.Text;
using Ogur.Hub.Domain.Common;

namespace Ogur.Hub.Domain.ValueObjects;

/// <summary>
/// Value object representing an API key with hashing for security.
/// </summary>
public sealed class ApiKey : ValueObject
{
    /// <summary>
    /// Gets the hashed API key (SHA256).
    /// </summary>
    public string HashedValue { get; private set; }

    private ApiKey(string hashedValue)
    {
        HashedValue = hashedValue;
    }

    /// <summary>
    /// Creates an ApiKey from a raw key value (hashes it).
    /// </summary>
    /// <param name="rawKey">The raw API key.</param>
    /// <returns>A new ApiKey with hashed value.</returns>
    public static ApiKey CreateFromRaw(string rawKey)
    {
        if (string.IsNullOrWhiteSpace(rawKey))
            throw new ArgumentException("API key cannot be empty", nameof(rawKey));

        var hashedValue = HashKey(rawKey);
        return new ApiKey(hashedValue);
    }

    /// <summary>
    /// Creates an ApiKey from an already hashed value.
    /// </summary>
    /// <param name="hashedValue">The hashed API key.</param>
    /// <returns>A new ApiKey instance.</returns>
    public static ApiKey CreateFromHash(string hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            throw new ArgumentException("Hashed value cannot be empty", nameof(hashedValue));

        return new ApiKey(hashedValue);
    }

    /// <summary>
    /// Generates a new random API key.
    /// </summary>
    /// <returns>A tuple containing the raw key and the ApiKey object.</returns>
    public static (string RawKey, ApiKey HashedKey) Generate()
    {
        var rawKey = GenerateRandomKey();
        var hashedKey = CreateFromRaw(rawKey);
        return (rawKey, hashedKey);
    }

    /// <summary>
    /// Verifies if a raw key matches this hashed key.
    /// </summary>
    /// <param name="rawKey">The raw key to verify.</param>
    /// <returns>True if the key matches, false otherwise.</returns>
    public bool Verify(string rawKey)
    {
        if (string.IsNullOrWhiteSpace(rawKey))
            return false;

        var hashedInput = HashKey(rawKey);
        return hashedInput == HashedValue;
    }

    private static string HashKey(string key)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(key);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static string GenerateRandomKey()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Gets the atomic values that define equality.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HashedValue;
    }

    /// <summary>
    /// Returns the hashed value.
    /// </summary>
    public override string ToString() => HashedValue;
}