// File: Ogur.Hub.Application/Commands/UsersCommands/DeleteUserCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.UsersCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;

namespace Ogur.Hub.Application.Commands.UsersCommands;

/// <summary>
/// Command to delete a user.
/// </summary>
/// <param name="UserId">User ID to delete.</param>
public sealed record DeleteUserCommand(int UserId);

/// <summary>
/// Handler for DeleteUserCommand.
/// </summary>
public sealed class DeleteUserCommandHandler
{
    private readonly IRepository<Domain.Entities.User, int> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public DeleteUserCommandHandler(
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
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, ct);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        _userRepository.Remove(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}