// File: Hub.Application/Commands/Users/UpdateUserCommand.cs
// Project: Hub.Application
// Namespace: Ogur.Hub.Application.Commands.Users

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;

namespace Ogur.Hub.Application.Commands.UsersCommands;

/// <summary>
/// Command to update user details.
/// </summary>
/// <param name="UserId">User ID.</param>
/// <param name="Email">New email address.</param>
/// <param name="IsActive">Is user active.</param>
/// <param name="IsAdmin">Is user admin.</param>
public sealed record UpdateUserCommand(
    int UserId,
    string Email,
    bool IsActive,
    bool IsAdmin);

/// <summary>
/// Handler for UpdateUserCommand.
/// </summary>
public sealed class UpdateUserCommandHandler
{
    private readonly IRepository<Domain.Entities.User, int> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public UpdateUserCommandHandler(
        IRepository<Domain.Entities.User, int> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">Command to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success or failure result.</returns>
    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, ct);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        // Update email
        user.Update(command.Email);

        // Update active status
        if (command.IsActive && !user.IsActive)
        {
            user.Activate();
        }
        else if (!command.IsActive && user.IsActive)
        {
            user.Deactivate();
        }

        // Note: IsAdmin is not updated for security reasons
        // Create separate command if needed

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}