// File: Ogur.Hub.Application/Commands/Applications/UpdateApplicationCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.Applications

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;
using ApplicationEntity = Ogur.Hub.Domain.Entities.Application;


namespace Ogur.Hub.Application.Commands.ApplicationsCommands;

/// <summary>
/// Command to update an application.
/// </summary>
/// <param name="ApplicationId">Application ID to update.</param>
/// <param name="DisplayName">New display name.</param>
/// <param name="Description">New description.</param>
public sealed record UpdateApplicationCommand(
    int ApplicationId,
    string DisplayName,
    string? Description = null);

/// <summary>
/// Handler for UpdateApplicationCommand.
/// </summary>
public sealed class UpdateApplicationCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IRepository<ApplicationEntity, int> _applicationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateApplicationCommandHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="applicationRepository">Application repository.</param>
    public UpdateApplicationCommandHandler(
        IApplicationDbContext context,
        IRepository<ApplicationEntity, int> applicationRepository)
    {
        _context = context;
        _applicationRepository = applicationRepository;
    }

    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">Command to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result indicating success or failure.</returns>
    public async Task<Result> Handle(UpdateApplicationCommand command, CancellationToken ct)
    {
        var application = await _applicationRepository.GetByIdAsync(command.ApplicationId, ct);
        if (application is null)
            return Result.Failure($"Application with ID {command.ApplicationId} not found");

        application.Update(command.DisplayName, command.Description);
        _applicationRepository.Update(application);
        await _context.SaveChangesAsync(ct);

        return Result.Success();
    }
}