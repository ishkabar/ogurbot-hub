// File: Ogur.Hub.Api/Controllers/LicensesController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Api.Models.Requests;
using Ogur.Hub.Api.Models.Responses;
using Ogur.Hub.Application.Commands.LicensesCommands;
using Ogur.Hub.Application.Queries.Licenses;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Api.Controllers;

/// <summary>
/// Controller for license management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class LicensesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LicensesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LicensesController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator instance.</param>
    /// <param name="logger">Logger instance.</param>
    public LicensesController(IMediator mediator, ILogger<LicensesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Validates a license and registers or updates the device.
    /// </summary>
    /// <param name="request">License validation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>License validation response.</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ApiResponse<LicenseValidationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ValidateLicense(
        [FromBody] ValidateLicenseRequest request,
        CancellationToken cancellationToken)
    {
        var applicationId = HttpContext.Items["ApplicationId"] as int?;
        if (applicationId is null)
        {
            return Unauthorized(ApiResponse<LicenseValidationResponse>.ErrorResponse("Application not authenticated"));
        }

        var command = new ValidateLicenseCommand(
            request.LicenseKey,
            applicationId.Value,
            request.Hwid,
            request.DeviceGuid,
            request.DeviceName);

        var result = await _mediator.Send(command, cancellationToken);

        var response = new LicenseValidationResponse
        {
            IsValid = result.IsValid,
            DeviceId = result.DeviceId,
            IsNewDevice = result.IsNewDevice,
            ExpiresAt = result.ExpiresAt,
            RegisteredDevices = result.RegisteredDevices,
            MaxDevices = result.MaxDevices,
            ErrorMessage = result.ErrorMessage
        };

        _logger.LogInformation(
            "License validation for key {LicenseKey} from app {ApplicationId}: {IsValid}",
            request.LicenseKey,
            applicationId,
            result.IsValid);

        return Ok(ApiResponse<LicenseValidationResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// Retrieves all licenses with optional filtering.
    /// </summary>
    /// <param name="applicationId">Optional application identifier filter.</param>
    /// <param name="userId">Optional user identifier filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of licenses.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<LicenseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLicenses(
        [FromQuery] int? applicationId,
        [FromQuery] int? userId,
        CancellationToken cancellationToken)
    {
        var query = new GetLicensesQuery(applicationId, userId);
        var licenses = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse<List<LicenseDto>>.SuccessResponse(licenses));
    }

    /// <summary>
    /// Creates a new license.
    /// </summary>
    /// <param name="request">License creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created license details.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<LicenseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateLicense(
        [FromBody] CreateLicenseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLicenseCommand(
            request.ApplicationId,
            request.UserId,
            request.MaxDevices,
            request.StartDate,
            request.EndDate);

        var license = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation(
            "License created for user {UserId} and application {ApplicationId}",
            request.UserId,
            request.ApplicationId);

        return CreatedAtAction(
            nameof(GetLicenses),
            new { },
            ApiResponse<LicenseDto>.SuccessResponse(license));
    }
}