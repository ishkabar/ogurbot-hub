// File: Ogur.Hub.Application/Queries/Users/GetUserByIdQuery.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Queries.Users

using Microsoft.EntityFrameworkCore;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Application.Queries.Users;

/// <summary>
/// Query to get user by ID.
/// </summary>
/// <param name="UserId">User ID.</param>
public sealed record GetUserByIdQuery(int UserId);

/// <summary>
/// Handler for GetUserByIdQuery.
/// </summary>
public sealed class GetUserByIdQueryHandler
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query.
    /// </summary>
    /// <param name="query">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>User details.</returns>
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var userWithLicenseCount = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == query.UserId)
            .GroupJoin(
                _context.Licenses,
                user => user.Id,
                license => license.UserId,
                (user, licenses) => new
                {
                    User = user,
                    LicenseCount = licenses.Count()
                })
            .FirstOrDefaultAsync(ct);

        if (userWithLicenseCount == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        var dto = new UserDto(
            userWithLicenseCount.User.Id,
            userWithLicenseCount.User.Username,
            userWithLicenseCount.User.Email,
            userWithLicenseCount.User.IsActive,
            userWithLicenseCount.User.Role,
            userWithLicenseCount.User.IsAdmin,
            userWithLicenseCount.LicenseCount,
            userWithLicenseCount.User.CreatedAt,
            userWithLicenseCount.User.LastLoginAt);

        return Result<UserDto>.Success(dto);
    }
}
