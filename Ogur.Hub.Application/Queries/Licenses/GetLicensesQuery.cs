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
                l.IsActive ? LicenseStatus.Active : LicenseStatus.Inactive,
                l.StartDate,
                l.EndDate,
                l.IsActive ? (DateTime?)null : l.UpdatedAt,
                null,
                null,
                0
            ))
            .ToListAsync(cancellationToken);

        return licenses;
    }
}