// File: Ogur.Hub.Application/Queries/Updates/CheckForUpdatesQuery.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Queries.Updates

using Microsoft.EntityFrameworkCore;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;
using ApplicationEntity = Ogur.Hub.Domain.Entities.Application;

namespace Ogur.Hub.Application.Queries.Updates;

/// <summary>
/// Query to check for application updates.
/// This is called by IUpdateChecker from Ogur.Core.
/// </summary>
/// <param name="ApplicationName">Application name.</param>
/// <param name="CurrentVersion">Current version.</param>
public sealed record CheckForUpdatesQuery(string ApplicationName, string CurrentVersion);

/// <summary>
/// Result of update check.
/// </summary>
/// <param name="IsUpdateAvailable">Whether update is available.</param>
/// <param name="LatestVersion">Latest version.</param>
/// <param name="DownloadUrl">Download URL.</param>
/// <param name="ReleaseNotes">Release notes.</param>
/// <param name="IsRequired">Whether update is required.</param>
public sealed record CheckForUpdatesResult(
    bool IsUpdateAvailable,
    string? LatestVersion,
    string? DownloadUrl,
    string? ReleaseNotes,
    bool IsRequired);

/// <summary>
/// Handler for CheckForUpdatesQuery.
/// </summary>
public sealed class CheckForUpdatesQueryHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IRepository<ApplicationEntity, int> _applicationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckForUpdatesQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="applicationRepository">Application repository.</param>
    public CheckForUpdatesQueryHandler(
        IApplicationDbContext context,
        IRepository<ApplicationEntity, int> applicationRepository)
    {
        _context = context;
        _applicationRepository = applicationRepository;
    }

    /// <summary>
    /// Handles the query.
    /// </summary>
    /// <param name="query">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Update check result.</returns>
    public async Task<Result<CheckForUpdatesResult>> Handle(CheckForUpdatesQuery query, CancellationToken ct)
    {
        // Find application
        var application = await _applicationRepository.FirstOrDefaultAsync(
            a => a.Name == query.ApplicationName, ct);

        if (application is null)
        {
            return Result<CheckForUpdatesResult>.Success(
                new CheckForUpdatesResult(false, null, null, null, false));
        }

        // Get latest version
        var latestVersion = await _context.ApplicationVersions
            .Where(v => v.ApplicationId == application.Id && v.IsLatest)
            .OrderByDescending(v => v.ReleasedAt)
            .FirstOrDefaultAsync(ct);

        if (latestVersion is null)
        {
            return Result<CheckForUpdatesResult>.Success(
                new CheckForUpdatesResult(false, application.CurrentVersion, null, null, false));
        }

        // Compare versions (simple string comparison, you might want semantic versioning)
        var isUpdateAvailable = latestVersion.Version != query.CurrentVersion;

        var result = new CheckForUpdatesResult(
            isUpdateAvailable,
            latestVersion.Version,
            latestVersion.DownloadUrl,
            latestVersion.ReleaseNotes,
            latestVersion.IsRequired);

        return Result<CheckForUpdatesResult>.Success(result);
    }
}