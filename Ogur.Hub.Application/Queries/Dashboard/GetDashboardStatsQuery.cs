// File: Ogur.Hub.Application/Queries/Dashboard/GetDashboardStatsQuery.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Queries.Dashboard

using MediatR;
using Microsoft.EntityFrameworkCore;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Application.Queries.Dashboard;

/// <summary>
/// Query to retrieve dashboard statistics
/// </summary>
public sealed record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

/// <summary>
/// Dashboard statistics data transfer object
/// </summary>
/// <param name="TotalApplications">Total number of applications</param>
/// <param name="ActiveLicenses">Number of active licenses</param>
/// <param name="ConnectedDevices">Number of currently connected devices</param>
/// <param name="CommandsToday">Number of commands sent today</param>
public sealed record DashboardStatsDto(
    int TotalApplications,
    int ActiveLicenses,
    int ConnectedDevices,
    int CommandsToday);

/// <summary>
/// Handler for retrieving dashboard statistics
/// </summary>
public sealed class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the GetDashboardStatsQueryHandler
    /// </summary>
    /// <param name="context">Database context</param>
    public GetDashboardStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var totalApplications = await _context.Applications
            .CountAsync(cancellationToken);
        
        var activeLicenses = await _context.Licenses
            .CountAsync(l => l.IsActive, cancellationToken);
        
        var connectedDevices = await _context.Devices
            .Include(d => d.Sessions)
            .CountAsync(d => d.Sessions.Any(s => s.DisconnectedAt == null) && d.Status == DeviceStatus.Warning, 
                cancellationToken);
        
        var today = DateTime.UtcNow.Date;
        var commandsToday = await _context.HubCommands
            .CountAsync(c => c.SentAt >= today, cancellationToken);

        return new DashboardStatsDto(
            totalApplications,
            activeLicenses,
            connectedDevices,
            commandsToday);
    }
}