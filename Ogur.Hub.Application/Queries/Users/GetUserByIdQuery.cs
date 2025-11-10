// File: Hub.Application/Queries/Users/GetUserByIdQuery.cs
// Project: Hub.Application
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
    /// <returns>User DTO or error if not found.</returns>
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == query.UserId)
            .Select(u => new UserDto(
                u.Id,
                u.Username,
                u.Email,
                u.IsActive,
                u.IsAdmin,
                _context.Licenses.Count(l => l.UserId == u.Id),
                u.CreatedAt,
                u.LastLoginAt))
            .FirstOrDefaultAsync(ct);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        return Result<UserDto>.Success(user);
    }
}