// File: Ogur.Hub.Api/Controllers/DevicesController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

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
    private readonly GetDevicesQueryHandler _getDevicesHandler;
    private readonly UpdateDeviceCommandHandler _updateDeviceHandler;
    private readonly AssignUserToDeviceCommandHandler _assignUserHandler;
    private readonly RemoveUserFromDeviceCommandHandler _removeUserHandler;
    private readonly BlockDeviceCommandHandler _blockDeviceHandler;
    private readonly UnblockDeviceCommandHandler _unblockDeviceHandler;
    private readonly SendDeviceCommandCommandHandler _sendCommandHandler;
    private readonly ILogger<DevicesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/> class.
    /// </summary>
    public DevicesController(
        GetDevicesQueryHandler getDevicesHandler,
        UpdateDeviceCommandHandler updateDeviceHandler,
        AssignUserToDeviceCommandHandler assignUserHandler,
        RemoveUserFromDeviceCommandHandler removeUserHandler,
        BlockDeviceCommandHandler blockDeviceHandler,
        UnblockDeviceCommandHandler unblockDeviceHandler,
        SendDeviceCommandCommandHandler sendCommandHandler,
        ILogger<DevicesController> logger)
    {
        _getDevicesHandler = getDevicesHandler;
        _updateDeviceHandler = updateDeviceHandler;
        _assignUserHandler = assignUserHandler;
        _removeUserHandler = removeUserHandler;
        _blockDeviceHandler = blockDeviceHandler;
        _unblockDeviceHandler = unblockDeviceHandler;
        _sendCommandHandler = sendCommandHandler;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all devices with optional license filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<DeviceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDevices(
        [FromQuery] int? licenseId,
        CancellationToken cancellationToken)
    {
        var query = new GetDevicesQuery(licenseId);
        var devices = await _getDevicesHandler.Handle(query, cancellationToken);

        return Ok(ApiResponse<List<DeviceDto>>.SuccessResponse(devices));
    }
    
    /// <summary>
    /// Updates device information.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateDevice(
        int id,
        [FromBody] UpdateDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDeviceCommand(
            id,
            request.DeviceName,
            request.Description,
            request.PrimaryUserId);

        var result = await _updateDeviceHandler.Handle(command, cancellationToken);

        if (!result)
        {
            return NotFound(ApiResponse<bool>.ErrorResponse("Device not found"));
        }

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    /// <summary>
    /// Assigns a user to a device.
    /// </summary>
    [HttpPost("{id}/users")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AssignUser(
        int id,
        [FromBody] AssignUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignUserToDeviceCommand(id, request.UserId);
        var result = await _assignUserHandler.Handle(command, cancellationToken);

        if (!result)
        {
            return NotFound(ApiResponse<bool>.ErrorResponse("Device not found"));
        }

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    /// <summary>
    /// Removes a user from a device.
    /// </summary>
    [HttpDelete("{id}/users/{userId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveUser(
        int id,
        int userId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveUserFromDeviceCommand(id, userId);
        var result = await _removeUserHandler.Handle(command, cancellationToken);

        if (!result)
        {
            return NotFound(ApiResponse<bool>.ErrorResponse("Device not found"));
        }

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    /// <summary>
    /// Blocks a device, preventing further access.
    /// </summary>
    [HttpPost("{id}/block")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BlockDevice(int id, CancellationToken cancellationToken)
    {
        var command = new BlockDeviceCommand(id);
        var result = await _blockDeviceHandler.Handle(command, cancellationToken);

        if (!result)
        {
            return NotFound(ApiResponse<bool>.ErrorResponse("Device not found"));
        }

        _logger.LogInformation("Device {DeviceId} has been blocked", id);

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }
    
    /// <summary>
    /// Unblocks a device.
    /// </summary>
    [HttpPost("{id}/unblock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnblockDevice(int id, CancellationToken cancellationToken)
    {
        var command = new UnblockDeviceCommand(id);
        var result = await _unblockDeviceHandler.Handle(command, cancellationToken);

        if (!result)
        {
            return NotFound(ApiResponse<bool>.ErrorResponse("Device not found"));
        }

        _logger.LogInformation("Device {DeviceId} has been unblocked", id);

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    /// <summary>
    /// Forces a device logout by sending a logout command.
    /// </summary>
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

        var commandId = await _sendCommandHandler.Handle(command, cancellationToken);

        _logger.LogInformation("Logout command sent to device {DeviceId}", id);

        return Ok(ApiResponse<int>.SuccessResponse(commandId));
    }

    /// <summary>
    /// Sends a custom command to a device.
    /// </summary>
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
        var commandId = await _sendCommandHandler.Handle(command, cancellationToken);

        _logger.LogInformation(
            "Command {CommandType} sent to device {DeviceId}",
            request.CommandType,
            id);

        return Ok(ApiResponse<int>.SuccessResponse(commandId));
    }
}