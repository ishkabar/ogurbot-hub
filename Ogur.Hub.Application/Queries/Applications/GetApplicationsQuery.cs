// File: Ogur.Hub.Application/Queries/Applications/GetApplicationsQuery.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Queries.Applications

using Microsoft.EntityFrameworkCore;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Application.Queries.Applications;

/// <summary>
/// Query to get all applications.
/// </summary>
public sealed record GetApplicationsQuery;

/// <summary>
/// Handler for GetApplicationsQuery.
/// </summary>
public sealed class GetApplicationsQueryHandler
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetApplicationsQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public GetApplicationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query.
    /// </summary>
    /// <param name="query">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of applications.</returns>
    public async Task<Result<IReadOnlyList<ApplicationDto>>> Handle(GetApplicationsQuery query, CancellationToken ct)
    {
        var applications = await _context.Applications
            .AsNoTracking()
            .Select(a => new ApplicationDto(
                a.Id,
                a.Name,
                a.DisplayName,
                a.Description,
                a.CurrentVersion,
                a.IsActive,
                a.CreatedAt,
                a.UpdatedAt))
            .ToListAsync(ct);

        return Result<IReadOnlyList<ApplicationDto>>.Success(applications);
    }
}