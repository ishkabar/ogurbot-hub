// File: Ogur.Hub.Application/Queries/Devices/GetDevicesQuery.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Queries.Devices

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.DTO;
using Ogur.Hub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ogur.Hub.Application.Queries.Devices;

/// <summary>
/// Query to retrieve devices with optional filtering by license.
/// </summary>
/// <param name="LicenseId">Optional license identifier to filter devices.</param>
public sealed record GetDevicesQuery(int? LicenseId = null) : IRequest<List<DeviceDto>>;

/// <summary>
/// Handler for retrieving devices.
/// </summary>
public sealed class GetDevicesQueryHandler : IRequestHandler<GetDevicesQuery, List<DeviceDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDevicesQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public GetDevicesQueryHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<List<DeviceDto>> Handle(GetDevicesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Devices
            .Include(d => d.License)
            .Include(d => d.PrimaryUser)
            .Include(d => d.Sessions.Where(s => s.DisconnectedAt == null))
            .AsQueryable();

        if (request.LicenseId.HasValue)
        {
            query = query.Where(d => d.LicenseId == request.LicenseId.Value);
        }

        var devices = await query.ToListAsync(cancellationToken);

        return devices.Select(d =>
        {
            var activeSession = d.Sessions.FirstOrDefault(s => s.DisconnectedAt == null);
        
            return new DeviceDto(
                d.Id,
                d.LicenseId,
                d.Fingerprint.Hwid,
                d.Fingerprint.DeviceGuid.ToString(),
                d.DeviceName ?? string.Empty,
                d.Description,
                d.PrimaryUserId,
                d.PrimaryUser?.Email,
                d.PrimaryUser?.Username,
                d.Status,
                d.LastIpAddress,
                d.RegisteredAt,
                d.LastSeenAt ?? d.RegisteredAt,
                null,
                null,
                activeSession?.ConnectionId,
                activeSession?.ConnectedAt
            );
        }).ToList();
    }
}