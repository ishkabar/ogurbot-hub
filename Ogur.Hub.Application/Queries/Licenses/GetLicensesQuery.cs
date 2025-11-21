// File: Ogur.Hub.Application/Queries/Licenses/GetLicensesQuery.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Queries.Licenses

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.DTO;
using Ogur.Hub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ogur.Hub.Application.Queries.Licenses;

/// <summary>
/// Query to retrieve licenses with optional filtering.
/// </summary>
/// <param name="ApplicationId">Optional application ID filter.</param>
/// <param name="UserId">Optional user ID filter.</param>
public sealed record GetLicensesQuery(int? ApplicationId = null, int? UserId = null) : IRequest<List<LicenseDto>>;

/// <summary>
/// Handler for retrieving licenses.
/// </summary>
public sealed class GetLicensesQueryHandler : IRequestHandler<GetLicensesQuery, List<LicenseDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLicensesQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public GetLicensesQueryHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<List<LicenseDto>> Handle(GetLicensesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Licenses
            .Include(l => l.Application)
            .Include(l => l.Devices)
            .AsQueryable();

        if (request.ApplicationId.HasValue)
        {
            query = query.Where(l => l.ApplicationId == request.ApplicationId.Value);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(l => l.UserId == request.UserId.Value);
        }

        var licenses = await query
            .Select(l => new LicenseDto(
                l.Id,
                l.LicenseKey.Value,
                l.ApplicationId,
                l.Application.DisplayName,
                l.UserId,
                l.MaxDevices,
                l.Devices.Count,
                l.Status,
                l.StartDate,
                l.EndDate,
                l.Status == LicenseStatus.Revoked ? l.UpdatedAt : null,
                null,
                null,
                0,
                l.Description
            ))
            .ToListAsync(cancellationToken);

        return licenses;
    }
}

/// <summary>
/// Query to get active license for user and application.
/// </summary>
/// <param name="UserId">User identifier.</param>
/// <param name="ApplicationId">Application identifier.</param>
public sealed record GetUserLicenseQuery(int UserId, int ApplicationId) : IRequest<LicenseDto?>;

/// <summary>
/// Handler for getting user's active license.
/// </summary>
public sealed class GetUserLicenseQueryHandler : IRequestHandler<GetUserLicenseQuery, LicenseDto?>
{
    private readonly IApplicationDbContext _context;

    public GetUserLicenseQueryHandler(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<LicenseDto?> Handle(GetUserLicenseQuery request, CancellationToken cancellationToken)
    {
        var license = await _context.Licenses
            .Include(l => l.Application)
            .Include(l => l.Devices)
            .Where(l => l.UserId == request.UserId
                        && l.ApplicationId == request.ApplicationId
                        && l.Status == LicenseStatus.Active
                        && (l.EndDate == null || l.EndDate > DateTime.UtcNow))
            .OrderByDescending(l => l.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (license is null)
            return null;

        return new LicenseDto(
            license.Id,
            license.LicenseKey.Value,
            license.ApplicationId,
            license.Application.DisplayName,
            license.UserId,
            license.MaxDevices,
            license.Devices.Count,
            license.Status,
            license.StartDate,
            license.EndDate,
            license.Status == LicenseStatus.Revoked ? license.UpdatedAt : null,
            null,
            null,
            0,
            license.Description
        );
    }
}