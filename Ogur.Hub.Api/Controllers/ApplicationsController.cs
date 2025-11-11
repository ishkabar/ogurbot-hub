// File: Ogur.Hub.Api/Controllers/ApplicationsController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Api.Models.Requests;
using Ogur.Hub.Api.Models.Responses;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using DomainApplication = Ogur.Hub.Domain.Entities.Application;

namespace Ogur.Hub.Api.Controllers;

/// <summary>
/// Controller for application management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public sealed class ApplicationsController : ControllerBase
{
    private readonly IRepository<DomainApplication, int> _applicationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApplicationsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationsController"/> class.
    /// </summary>
    /// <param name="applicationRepository">Application repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="logger">Logger instance.</param>
    public ApplicationsController(
        IRepository<DomainApplication, int> applicationRepository,
        IUnitOfWork unitOfWork,
        ILogger<ApplicationsController> logger)
    {
        _applicationRepository = applicationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all registered applications.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of applications.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ApplicationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetApplications(CancellationToken cancellationToken)
    {
        var applications = await _applicationRepository.GetAllAsync(cancellationToken);

        var dtos = applications.Select(a => new ApplicationDto(
            a.Id,
            a.Name,
            a.DisplayName,
            a.Description,
            a.CurrentVersion,
            a.IsActive,
            a.CreatedAt)).ToList();

        return Ok(ApiResponse<List<ApplicationDto>>.SuccessResponse(dtos));
    }

    /// <summary>
    /// Retrieves a specific application by identifier.
    /// </summary>
    /// <param name="id">Application identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Application details.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetApplication(int id, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdAsync(id, cancellationToken);

        if (application is null)
        {
            return NotFound(ApiResponse<ApplicationDto>.ErrorResponse("Application not found"));
        }

        var dto = new ApplicationDto(
            application.Id,
            application.Name,
            application.DisplayName,
            application.Description,
            application.CurrentVersion,
            application.IsActive,
            application.CreatedAt);

        return Ok(ApiResponse<ApplicationDto>.SuccessResponse(dto));
    }

    /// <summary>
    /// Creates a new application and generates an API key.
    /// </summary>
    /// <param name="request">Application creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created application details including raw API key.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateApplicationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateApplication(
        [FromBody] CreateApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var existingApplications = await _applicationRepository.GetAllAsync(cancellationToken);
        if (existingApplications.Any(a => a.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest(
                ApiResponse<CreateApplicationResponse>.ErrorResponse("Application with this name already exists"));
        }

        var (application, rawApiKey) = DomainApplication.Create(
            request.Name,
            request.DisplayName,
            request.CurrentVersion,
            request.Description);

        await _applicationRepository.AddAsync(application, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Application {ApplicationName} created with ID {ApplicationId}", application.Name,
            application.Id);

        var response = new CreateApplicationResponse
        {
            Id = application.Id,
            Name = application.Name,
            DisplayName = application.DisplayName,
            Description = application.Description,
            CurrentVersion = application.CurrentVersion,
            ApiKey = rawApiKey,
            IsActive = application.IsActive,
            CreatedAt = application.CreatedAt
        };

        return CreatedAtAction(
            nameof(GetApplication),
            new { id = application.Id },
            ApiResponse<CreateApplicationResponse>.SuccessResponse(response));
    }


    /// <summary>
    /// Updates an existing application.
    /// </summary>
    /// <param name="id">Application identifier.</param>
    /// <param name="request">Application update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Update result.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateApplication(
        int id,
        [FromBody] UpdateApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdAsync(id, cancellationToken);

        if (application is null)
        {
            return NotFound(ApiResponse<ApplicationDto>.ErrorResponse("Application not found"));
        }

        // Check for name conflict with other applications
        var existingApplications = await _applicationRepository.GetAllAsync(cancellationToken);
        if (existingApplications.Any(a =>
                a.Id != id && a.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest(ApiResponse<ApplicationDto>.ErrorResponse("Application with this name already exists"));
        }

        application.Update(
            request.Name,
            request.DisplayName,
            request.CurrentVersion,
            request.Description,
            request.IsActive);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Application {ApplicationId} updated by admin", application.Id);

        var dto = new ApplicationDto(
            application.Id,
            application.Name,
            application.DisplayName,
            application.Description,
            application.CurrentVersion,
            application.IsActive,
            application.CreatedAt);

        return Ok(ApiResponse<ApplicationDto>.SuccessResponse(dto));
    }

    /// <summary>
    /// Regenerates the API key for an application.
    /// </summary>
    /// <param name="id">Application identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>New API key.</returns>
    [HttpPost("{id}/regenerate-key")]
    [ProducesResponseType(typeof(ApiResponse<RegenerateApiKeyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegenerateApiKey(int id, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdAsync(id, cancellationToken);

        if (application is null)
        {
            return NotFound(ApiResponse<RegenerateApiKeyResponse>.ErrorResponse("Application not found"));
        }

        var newApiKey = application.RegenerateApiKey();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogWarning("API key regenerated for application {ApplicationName}", application.Name);

        var response = new RegenerateApiKeyResponse
        {
            ApplicationId = application.Id,
            NewApiKey = newApiKey
        };

        return Ok(ApiResponse<RegenerateApiKeyResponse>.SuccessResponse(response));
    }
}

/// <summary>
/// DTO for application information.
/// </summary>
public sealed record ApplicationDto(
    int Id,
    string Name,
    string DisplayName,
    string? Description,
    string CurrentVersion,
    bool IsActive,
    DateTime CreatedAt);

/// <summary>
/// Response for application creation including raw API key.
/// </summary>
public sealed record CreateApplicationResponse
{
    /// <summary>
    /// Application identifier.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Application name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Current version.
    /// </summary>
    public required string CurrentVersion { get; init; }

    /// <summary>
    /// Raw API key (only shown once).
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Whether the application is active.
    /// </summary>
    public required bool IsActive { get; init; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}

/// <summary>
/// Response for API key regeneration.
/// </summary>
public sealed record RegenerateApiKeyResponse
{
    /// <summary>
    /// Application identifier.
    /// </summary>
    public required int ApplicationId { get; init; }

    /// <summary>
    /// New raw API key.
    /// </summary>
    public required string NewApiKey { get; init; }
}