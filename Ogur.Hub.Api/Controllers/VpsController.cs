// File: Ogur.Hub.Api/Controllers/VpsController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using Ogur.Hub.Application.DTO;
using Ogur.Hub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ogur.hub.Api.Controllers;

/// <summary>
/// Controller for VPS monitoring endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VpsController : ControllerBase
{
    private readonly IVpsMonitorService _vpsMonitor;
    private readonly ILogger<VpsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="VpsController"/> class.
    /// </summary>
    /// <param name="vpsMonitor">VPS monitoring service.</param>
    /// <param name="logger">Logger instance.</param>
    public VpsController(IVpsMonitorService vpsMonitor, ILogger<VpsController> logger)
    {
        _vpsMonitor = vpsMonitor;
        _logger = logger;
    }

    /// <summary>
    /// Gets all Docker containers with their current stats.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of containers.</returns>
    [HttpGet("containers")]
    public async Task<IActionResult> GetContainers(CancellationToken cancellationToken)
    {
        var containers = await _vpsMonitor.GetContainersAsync(cancellationToken);
        return Ok(containers);
    }

    /// <summary>
    /// Gets all configured websites.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of websites.</returns>
    [HttpGet("websites")]
    public async Task<IActionResult> GetWebsites(CancellationToken cancellationToken)
    {
        var websites = await _vpsMonitor.GetWebsitesAsync(cancellationToken);
        return Ok(websites);
    }

    /// <summary>
    /// Gets current VPS resource usage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Current resource snapshot.</returns>
    [HttpGet("resources/current")]
    [ProducesResponseType(typeof(VpsResourceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentResources(CancellationToken cancellationToken)
    {
        try
        {
            var resources = await _vpsMonitor.GetCurrentResourcesAsync(cancellationToken);
            return Ok(resources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current resources");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets historical resource snapshots.
    /// </summary>
    /// <param name="from">Start timestamp.</param>
    /// <param name="to">End timestamp.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of resource snapshots.</returns>
    [HttpGet("resources/history")]
    public async Task<IActionResult> GetResourceHistory([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken cancellationToken)
    {
        var history = await _vpsMonitor.GetResourceHistoryAsync(from, to, cancellationToken);
        return Ok(history);
    }

    /// <summary>
    /// Refreshes container statistics from Docker.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Action result.</returns>
    [HttpPost("containers/refresh")]
    public async Task<IActionResult> RefreshContainers(CancellationToken cancellationToken)
    {
        await _vpsMonitor.RefreshContainerStatsAsync(cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Refreshes website health checks.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Action result.</returns>
    [HttpPost("websites/refresh")]
    public async Task<IActionResult> RefreshWebsites(CancellationToken cancellationToken)
    {
        await _vpsMonitor.RefreshWebsiteHealthAsync(cancellationToken);
        return Ok();
    }
    
    /// <summary>
    /// Adds a new website to monitor.
    /// </summary>
    /// <param name="dto">Website data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Added website.</returns>
    [HttpPost("websites")]
    public async Task<IActionResult> AddWebsite([FromBody] AddWebsiteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var website = await _vpsMonitor.AddWebsiteAsync(dto, cancellationToken);
            return Ok(website);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add website {Domain}", dto.Domain);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a website.
    /// </summary>
    /// <param name="id">Website ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Action result.</returns>
    [HttpDelete("websites/{id}")]
    public async Task<IActionResult> DeleteWebsite(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _vpsMonitor.DeleteWebsiteAsync(id, cancellationToken);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Website {Id} not found", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete website {Id}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}