// File: Ogur.Hub.Domain/Entities/ApplicationVersion.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

using Ogur.Hub.Domain.Common;

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a version release of an application.
/// </summary>
public sealed class ApplicationVersion : Entity<int>
{
    /// <summary>
    /// Gets the application ID this version belongs to.
    /// </summary>
    public int ApplicationId { get; private set; }

    /// <summary>
    /// Gets the version string (e.g., "1.2.3").
    /// </summary>
    public string Version { get; private set; }

    /// <summary>
    /// Gets the release notes.
    /// </summary>
    public string? ReleaseNotes { get; private set; }

    /// <summary>
    /// Gets the download URL for this version.
    /// </summary>
    public string? DownloadUrl { get; private set; }

    /// <summary>
    /// Gets whether this update is required (forces update).
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// Gets when this version was released.
    /// </summary>
    public DateTime ReleasedAt { get; private set; }

    /// <summary>
    /// Gets whether this is the latest version for the application.
    /// </summary>
    public bool IsLatest { get; private set; }

    /// <summary>
    /// Gets the navigation property to the application.
    /// </summary>
    public Application Application { get; private set; } = null!;

    private ApplicationVersion() { }

    private ApplicationVersion(
        int applicationId,
        string version,
        string? releaseNotes,
        string? downloadUrl,
        bool isRequired)
    {
        ApplicationId = applicationId;
        Version = version;
        ReleaseNotes = releaseNotes;
        DownloadUrl = downloadUrl;
        IsRequired = isRequired;
        ReleasedAt = DateTime.UtcNow;
        IsLatest = true;
    }

    /// <summary>
    /// Creates a new application version.
    /// </summary>
    /// <param name="applicationId">Application ID.</param>
    /// <param name="version">Version string.</param>
    /// <param name="releaseNotes">Release notes.</param>
    /// <param name="downloadUrl">Download URL.</param>
    /// <param name="isRequired">Whether update is required.</param>
    /// <returns>A new ApplicationVersion instance.</returns>
    public static ApplicationVersion Create(
        int applicationId,
        string version,
        string? releaseNotes = null,
        string? downloadUrl = null,
        bool isRequired = false)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version is required", nameof(version));

        return new ApplicationVersion(applicationId, version, releaseNotes, downloadUrl, isRequired);
    }

    /// <summary>
    /// Marks this version as no longer the latest.
    /// </summary>
    public void MarkAsOutdated()
    {
        IsLatest = false;
    }

    /// <summary>
    /// Updates the release notes and download URL.
    /// </summary>
    /// <param name="releaseNotes">New release notes.</param>
    /// <param name="downloadUrl">New download URL.</param>
    public void Update(string? releaseNotes, string? downloadUrl)
    {
        ReleaseNotes = releaseNotes;
        DownloadUrl = downloadUrl;
    }

    /// <summary>
    /// Sets whether this update is required.
    /// </summary>
    /// <param name="isRequired">Required flag.</param>
    public void SetRequired(bool isRequired)
    {
        IsRequired = isRequired;
    }
}