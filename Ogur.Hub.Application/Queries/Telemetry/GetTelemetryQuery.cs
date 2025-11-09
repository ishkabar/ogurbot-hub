// File: Ogur.Hub.Application/Queries/Telemetry/GetTelemetryQuery.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Queries.Telemetry

using Microsoft.EntityFrameworkCore;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Application.Queries.Telemetry;

/// <summary>
/// Query to get telemetry data with filtering.
/// </summary>
/// <param name="DeviceId">Optional device ID filter.</param>
/// <param name="EventType">Optional event type filter.</param>
/// <param name="From">Optional start date filter.</param>
/// <param name="To">Optional end date filter.</param>
/// <param name="Take">Number of records to take (default: 100).</param>
public sealed record GetTelemetryQuery(
    int? DeviceId = null,
    string? EventType = null,
    DateTime? From = null,
    DateTime? To = null,
    int Take = 100);

/// <summary>
/// Handler for GetTelemetryQuery.
/// </summary>
public sealed class GetTelemetryQueryHandler
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTelemetryQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public GetTelemetryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query.
    /// </summary>
    /// <param name="query">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of telemetry entries.</returns>
    public async Task<Result<IReadOnlyList<TelemetryDto>>> Handle(GetTelemetryQuery query, CancellationToken ct)
    {
        var telemetryQuery = _context.Telemetry.AsNoTracking();

        if (query.DeviceId.HasValue)
            telemetryQuery = telemetryQuery.Where(t => t.DeviceId == query.DeviceId.Value);

        if (!string.IsNullOrWhiteSpace(query.EventType))
            telemetryQuery = telemetryQuery.Where(t => t.EventType == query.EventType);

        if (query.From.HasValue)
            telemetryQuery = telemetryQuery.Where(t => t.OccurredAt >= query.From.Value);

        if (query.To.HasValue)
            telemetryQuery = telemetryQuery.Where(t => t.OccurredAt <= query.To.Value);

        var telemetry = await telemetryQuery
            .OrderByDescending(t => t.ReceivedAt)
            .Take(query.Take)
            .Select(t => new TelemetryDto(
                t.Id,
                t.DeviceId,
                t.EventType,
                t.EventDataJson,
                t.OccurredAt,
                t.ReceivedAt))
            .ToListAsync(ct);

        return Result<IReadOnlyList<TelemetryDto>>.Success(telemetry);
    }
}