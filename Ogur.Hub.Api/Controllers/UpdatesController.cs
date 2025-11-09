// File: Ogur.Hub.Api/Controllers/UpdatesController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Api.Models.Responses;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Api.Controllers;

/// <summary>
/// Controller for checking application updates.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class UpdatesController : ControllerBase
{
    private readonly IRepository<ApplicationVersion, int> _versionRepository;
    private readonly ILogger<UpdatesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatesController"/> class.
    /// </summary>
    /// <param name="versionRepository">Application version repository.</param>
    /// <param name="logger">Logger instance.</param>
    public UpdatesController(
        IRepository<ApplicationVersion, int> versionRepository,
        ILogger<UpdatesController> logger)
    {
        _versionRepository = versionRepository;
        _logger = logger;
    }

    /// <summary>
    /// Checks for available application updates.
    /// </summary>
    /// <param name="currentVersion">Current version of the application.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Update information if available.</returns>
    [HttpGet("check")]
    [ProducesResponseType(typeof(ApiResponse<UpdateCheckResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckForUpdates(
        [FromQuery] string currentVersion,
        CancellationToken cancellationToken)
    {
        var applicationId = HttpContext.Items["ApplicationId"] as int?;
        if (applicationId is null)
        {
            return Unauthorized(ApiResponse<UpdateCheckResponse>.ErrorResponse("Application not authenticated"));
        }

        var versions = await _versionRepository.GetAllAsync(cancellationToken);
        var latestVersion = versions
            .Where(v => v.ApplicationId == applicationId.Value)
            .OrderByDescending(v => v.ReleasedAt)
            .FirstOrDefault();

        if (latestVersion is null)
        {
            var response = new UpdateCheckResponse
            {
                UpdateAvailable = false,
                CurrentVersion = currentVersion
            };

            return Ok(ApiResponse<UpdateCheckResponse>.SuccessResponse(response));
        }

        var updateAvailable = !latestVersion.Version.Equals(currentVersion, StringComparison.OrdinalIgnoreCase);

        var updateResponse = new UpdateCheckResponse
        {
            UpdateAvailable = updateAvailable,
            CurrentVersion = currentVersion,
            LatestVersion = latestVersion.Version,
            ReleaseNotes = latestVersion.ReleaseNotes,
            DownloadUrl = latestVersion.DownloadUrl,
            IsRequired = latestVersion.IsRequired,
            ReleasedAt = latestVersion.ReleasedAt
        };

        _logger.LogInformation(
            "Update check for application {ApplicationId}: current={CurrentVersion}, latest={LatestVersion}, available={UpdateAvailable}",
            applicationId,
            currentVersion,
            latestVersion.Version,
            updateAvailable);

        return Ok(ApiResponse<UpdateCheckResponse>.SuccessResponse(updateResponse));
    }
}

/// <summary>
/// Response for update check.
/// </summary>
public sealed record UpdateCheckResponse
{
    /// <summary>
    /// Whether an update is available.
    /// </summary>
    public required bool UpdateAvailable { get; init; }

    /// <summary>
    /// Current version reported by the client.
    /// </summary>
    public required string CurrentVersion { get; init; }

    /// <summary>
    /// Latest available version.
    /// </summary>
    public string? LatestVersion { get; init; }

    /// <summary>
    /// Release notes for the latest version.
    /// </summary>
    public string? ReleaseNotes { get; init; }

    /// <summary>
    /// Download URL for the update.
    /// </summary>
    public string? DownloadUrl { get; init; }

    /// <summary>
    /// Whether the update is required.
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// When the latest version was released.
    /// </summary>
    public DateTime? ReleasedAt { get; init; }
}