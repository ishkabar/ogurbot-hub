// File: Ogur.Hub.Api/Controllers/AuditController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Api.Models.Responses;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Api.Controllers;

/// <summary>
/// Controller for querying audit logs.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public sealed class AuditController : ControllerBase
{
    private readonly IRepository<AuditLog, int> _auditLogRepository;
    private readonly ILogger<AuditController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditController"/> class.
    /// </summary>
    /// <param name="auditLogRepository">Audit log repository.</param>
    /// <param name="logger">Logger instance.</param>
    public AuditController(
        IRepository<AuditLog, int> auditLogRepository,
        ILogger<AuditController> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves audit logs with optional filtering.
    /// </summary>
    /// <param name="userId">Optional user identifier filter.</param>
    /// <param name="entityType">Optional entity type filter.</param>
    /// <param name="action">Optional action filter.</param>
    /// <param name="from">Optional start date filter.</param>
    /// <param name="to">Optional end date filter.</param>
    /// <param name="limit">Maximum number of results (default 100, max 1000).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of audit log entries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AuditLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int? userId,
        [FromQuery] string? entityType,
        [FromQuery] string? action,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int limit = 100,
        CancellationToken cancellationToken = default)
    {
        if (limit < 1 || limit > 1000)
        {
            return BadRequest(ApiResponse<List<AuditLogDto>>.ErrorResponse("Limit must be between 1 and 1000"));
        }

        var allLogs = await _auditLogRepository.GetAllAsync(cancellationToken);

        var query = allLogs.AsEnumerable();

        if (userId.HasValue)
        {
            query = query.Where(log => log.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(log => log.EntityType != null && 
                log.EntityType.Equals(entityType, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(log => log.Action.Contains(action, StringComparison.OrdinalIgnoreCase));
        }

        if (from.HasValue)
        {
            query = query.Where(log => log.OccurredAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(log => log.OccurredAt <= to.Value);
        }

        var logs = query
            .OrderByDescending(log => log.OccurredAt)
            .Take(limit)
            .Select(log => new AuditLogDto(
                log.Id,
                log.UserId,
                log.Action,
                log.EntityType,
                log.EntityId,
                log.DetailsJson,
                log.IpAddress,
                log.OccurredAt))
            .ToList();

        return Ok(ApiResponse<List<AuditLogDto>>.SuccessResponse(logs));
    }

    /// <summary>
    /// Retrieves a specific audit log entry by identifier.
    /// </summary>
    /// <param name="id">Audit log identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Audit log entry details.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAuditLog(int id, CancellationToken cancellationToken)
    {
        var log = await _auditLogRepository.GetByIdAsync(id, cancellationToken);

        if (log is null)
        {
            return NotFound(ApiResponse<AuditLogDto>.ErrorResponse("Audit log not found"));
        }

        var dto = new AuditLogDto(
            log.Id,
            log.UserId,
            log.Action,
            log.EntityType,
            log.EntityId,
            log.DetailsJson,
            log.IpAddress,
            log.OccurredAt);

        return Ok(ApiResponse<AuditLogDto>.SuccessResponse(dto));
    }
}

/// <summary>
/// DTO for audit log information.
/// </summary>
public sealed record AuditLogDto(
    int Id,
    int? UserId,
    string Action,
    string? EntityType,
    int? EntityId,
    string? DetailsJson,
    string? IpAddress,
    DateTime OccurredAt);