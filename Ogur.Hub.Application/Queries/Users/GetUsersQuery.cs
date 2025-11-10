// File: Hub.Application/Queries/Users/GetUsersQuery.cs
// Project: Hub.Application
// Namespace: Ogur.Hub.Application.Queries.Users

using Microsoft.EntityFrameworkCore;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Application.Queries.Users;

/// <summary>
/// Query to get all users.
/// </summary>
public sealed record GetUsersQuery;

/// <summary>
/// Handler for GetUsersQuery.
/// </summary>
public sealed class GetUsersQueryHandler
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUsersQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query.
    /// </summary>
    /// <param name="query">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of users.</returns>
    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        // First, get users with their license counts using a simpler query
        var usersWithLicenseCounts = await _context.Users
            .AsNoTracking()
            .GroupJoin(
                _context.Licenses,
                user => user.Id,
                license => license.UserId,
                (user, licenses) => new
                {
                    User = user,
                    LicenseCount = licenses.Count()
                })
            .OrderBy(x => x.User.Username)
            .ToListAsync(ct);

        // Then map to DTOs in memory
        var users = usersWithLicenseCounts
            .Select(x => new UserDto(
                x.User.Id,
                x.User.Username,
                x.User.Email,
                x.User.IsActive,
                x.User.IsAdmin,
                x.LicenseCount,
                x.User.CreatedAt,
                x.User.LastLoginAt))
            .ToList();

        return Result<IReadOnlyList<UserDto>>.Success(users);
    }
}