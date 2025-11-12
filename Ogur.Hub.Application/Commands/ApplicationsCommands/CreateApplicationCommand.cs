// File: Ogur.Hub.Application/Commands/Applications/CreateApplicationCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.Applications

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;
using ApplicationEntity = Ogur.Hub.Domain.Entities.Application;


namespace Ogur.Hub.Application.Commands.ApplicationsCommands;

/// <summary>
/// Command to create a new application.
/// </summary>
/// <param name="Name">Application name.</param>
/// <param name="DisplayName">Display name.</param>
/// <param name="CurrentVersion">Current version.</param>
/// <param name="Description">Optional description.</param>
/// <param name="IsActive">Whether application is active.</param>
public sealed record CreateApplicationCommand(
    string Name,
    string DisplayName,
    string CurrentVersion,
    string? Description = null,
    bool IsActive = true);

/// <summary>
/// Result of creating an application.
/// </summary>
/// <param name="ApplicationId">Created application ID.</param>
/// <param name="ApiKey">Generated API key (raw, not hashed).</param>
public sealed record CreateApplicationResult(int ApplicationId, string ApiKey);

/// <summary>
/// Handler for CreateApplicationCommand.
/// </summary>
public sealed class CreateApplicationCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IRepository<ApplicationEntity, int> _applicationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateApplicationCommandHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="applicationRepository">Application repository.</param>
    public CreateApplicationCommandHandler(
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
    /// <returns>Result with application ID and API key.</returns>
    public async Task<Result<CreateApplicationResult>> Handle(CreateApplicationCommand command, CancellationToken ct)
    {
        // Check if application with same name already exists
        var exists = await _applicationRepository.AnyAsync(a => a.Name == command.Name, ct);
        if (exists)
            return Result<CreateApplicationResult>.Failure($"Application with name '{command.Name}' already exists");

        // Create application
        var (application, rawApiKey) = ApplicationEntity.Create(
            command.Name,
            command.DisplayName,
            command.CurrentVersion,
            command.Description);
            
            if (!command.IsActive)
            application.Deactivate();

        await _applicationRepository.AddAsync(application, ct);
        await _context.SaveChangesAsync(ct);

        var result = new CreateApplicationResult(application.Id, rawApiKey);
        return Result<CreateApplicationResult>.Success(result);
    }
}