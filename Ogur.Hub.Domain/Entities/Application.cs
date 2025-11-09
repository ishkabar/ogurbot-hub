// File: Ogur.Hub.Domain/Entities/Application.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

using Ogur.Hub.Domain.Common;
using Ogur.Hub.Domain.ValueObjects;

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a registered application in the Ogur Hub (e.g., Ogur.Sentinel, Monitor.App).
/// </summary>
public sealed class Application : AggregateRoot<int>
{
    /// <summary>
    /// Gets the unique application name (used in API calls).
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the display name for the application.
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// Gets the optional description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the current version of the application.
    /// </summary>
    public string CurrentVersion { get; private set; }

    /// <summary>
    /// Gets the API key used to authenticate this application's requests.
    /// </summary>
    public ApiKey ApiKey { get; private set; }

    /// <summary>
    /// Gets whether the application is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets when the application was registered.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets when the application was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    private Application() { }

    private Application(
        string name,
        string displayName,
        string currentVersion,
        ApiKey apiKey,
        string? description = null)
    {
        Name = name;
        DisplayName = displayName;
        CurrentVersion = currentVersion;
        ApiKey = apiKey;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new application with generated API key.
    /// </summary>
    /// <param name="name">Unique application name.</param>
    /// <param name="displayName">Display name.</param>
    /// <param name="currentVersion">Current version.</param>
    /// <param name="description">Optional description.</param>
    /// <returns>A tuple containing the Application and the raw API key.</returns>
    public static (Application Application, string RawApiKey) Create(
        string name,
        string displayName,
        string currentVersion,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Application name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name is required", nameof(displayName));

        if (string.IsNullOrWhiteSpace(currentVersion))
            throw new ArgumentException("Version is required", nameof(currentVersion));

        var (rawKey, hashedKey) = ApiKey.Generate();
        var application = new Application(name, displayName, currentVersion, hashedKey, description);

        return (application, rawKey);
    }

    /// <summary>
    /// Updates the application version.
    /// </summary>
    /// <param name="newVersion">New version string.</param>
    public void UpdateVersion(string newVersion)
    {
        if (string.IsNullOrWhiteSpace(newVersion))
            throw new ArgumentException("Version cannot be empty", nameof(newVersion));

        CurrentVersion = newVersion;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates application details.
    /// </summary>
    /// <param name="displayName">New display name.</param>
    /// <param name="description">New description.</param>
    public void Update(string displayName, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name is required", nameof(displayName));

        DisplayName = displayName;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Regenerates the API key for this application.
    /// </summary>
    /// <returns>The new raw API key (must be saved before returning to user).</returns>
    public string RegenerateApiKey()
    {
        var (rawKey, hashedKey) = ApiKey.Generate();
        ApiKey = hashedKey;
        UpdatedAt = DateTime.UtcNow;
        return rawKey;
    }

    /// <summary>
    /// Activates the application.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the application.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifies if a raw API key matches this application's key.
    /// </summary>
    /// <param name="rawKey">The raw API key to verify.</param>
    /// <returns>True if the key matches, false otherwise.</returns>
    public bool VerifyApiKey(string rawKey)
    {
        return ApiKey.Verify(rawKey);
    }
}