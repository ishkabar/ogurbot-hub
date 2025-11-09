// File: Ogur.Hub.Api/Controllers/DevicesController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Api.Models.Requests;
using Ogur.Hub.Api.Models.Responses;
using Ogur.Hub.Application.Commands.DevicesCommands;
using Ogur.Hub.Application.Queries.Devices;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Api.Controllers;

/// <summary>
/// Controller for device management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DevicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DevicesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator instance.</param>
    /// <param name="logger">Logger instance.</param>
    public DevicesController(IMediator mediator, ILogger<DevicesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all devices with optional license filtering.
    /// </summary>
    /// <param name="licenseId">Optional license identifier filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of devices.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<DeviceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDevices(
        [FromQuery] int? licenseId,
        CancellationToken cancellationToken)
    {
        var query = new GetDevicesQuery(licenseId);
        var devices = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<List<DeviceDto>>.SuccessResponse(devices));
    }

    /// <summary>
    /// Blocks a device, preventing further access.
    /// </summary>
    /// <param name="id">Device identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("{id}/block")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BlockDevice(int id, CancellationToken cancellationToken)
    {
        var command = new BlockDeviceCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
        {
            return NotFound(ApiResponse<bool>.ErrorResponse("Device not found"));
        }

        _logger.LogInformation("Device {DeviceId} has been blocked", id);

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    /// <summary>
    /// Forces a device logout by sending a logout command.
    /// </summary>
    /// <param name="id">Device identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Command identifier.</returns>
    [HttpPost("{id}/logout")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> LogoutDevice(int id, CancellationToken cancellationToken)
    {
        var command = new SendDeviceCommandCommand(
            id,
            Domain.Enums.CommandType.Logout,
            "{}");

        var commandId = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Logout command sent to device {DeviceId}", id);

        return Ok(ApiResponse<int>.SuccessResponse(commandId));
    }

    /// <summary>
    /// Sends a custom command to a device.
    /// </summary>
    /// <param name="id">Device identifier.</param>
    /// <param name="request">Command request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Command identifier.</returns>
    [HttpPost("{id}/command")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendCommand(
        int id,
        [FromBody] SendCommandRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SendDeviceCommandCommand(id, request.CommandType, request.Payload);
        var commandId = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation(
            "Command {CommandType} sent to device {DeviceId}",
            request.CommandType,
            id);

        return Ok(ApiResponse<int>.SuccessResponse(commandId));
    }
}