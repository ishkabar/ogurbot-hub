// File: Hub.Api/Controllers/DashboardController.cs
// Project: Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Application.Queries.Dashboard;

namespace Ogur.Hub.Api.Controllers;

/// <summary>
/// Dashboard statistics endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DashboardController> _logger;

    /// <summary>
    /// Initializes a new instance of the DashboardController
    /// </summary>
    /// <param name="mediator">Mediator instance</param>
    /// <param name="logger">Logger instance</param>
    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets dashboard statistics
    /// </summary>
    /// <returns>Dashboard statistics</returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStatsDto), 200)]
    public async Task<IActionResult> GetStats()
    {
        var query = new GetDashboardStatsQuery();
        var stats = await _mediator.Send(query);
        return Ok(stats);
    }
}