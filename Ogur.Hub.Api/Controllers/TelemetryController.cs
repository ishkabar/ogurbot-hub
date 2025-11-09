// File: Ogur.Hub.Api/Controllers/TelemetryController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Api.Models.Requests;
using Ogur.Hub.Api.Models.Responses;
using Ogur.Hub.Application.Commands.TelemetryCommands;

namespace Ogur.Hub.Api.Controllers;

/// <summary>
/// Controller for receiving telemetry data from applications.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class TelemetryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TelemetryController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator instance.</param>
    /// <param name="logger">Logger instance.</param>
    public TelemetryController(IMediator mediator, ILogger<TelemetryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Receives a batch of telemetry events from an application.
    /// </summary>
    /// <param name="deviceId">Device identifier from query string.</param>
    /// <param name="request">Telemetry batch request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReceiveTelemetry(
        [FromQuery] int deviceId,
        [FromBody] TelemetryBatchRequest request,
        CancellationToken cancellationToken)
    {
        var applicationId = HttpContext.Items["ApplicationId"] as int?;
        if (applicationId is null)
        {
            return Unauthorized(ApiResponse<bool>.ErrorResponse("Application not authenticated"));
        }

        if (request.Events is null || !request.Events.Any())
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse("No telemetry events provided"));
        }

        foreach (var telemetryEvent in request.Events)
        {
            var command = new ReceiveTelemetryCommand(
                deviceId,
                telemetryEvent.EventType,
                telemetryEvent.EventDataJson,
                telemetryEvent.OccurredAt);

            await _mediator.Send(command, cancellationToken);
        }

        _logger.LogInformation(
            "Received {EventCount} telemetry events from device {DeviceId}",
            request.Events.Count,
            deviceId);

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }
}