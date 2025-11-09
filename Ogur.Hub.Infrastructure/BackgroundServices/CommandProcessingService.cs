// File: Ogur.Hub.Infrastructure/BackgroundServices/CommandProcessingService.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.BackgroundServices

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using Ogur.Hub.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ogur.Hub.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that processes pending commands and dispatches them to connected devices.
/// </summary>
public sealed class CommandProcessingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CommandProcessingService> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandProcessingService"/> class.
    /// </summary>
    /// <param name="scopeFactory">Service scope factory for creating scoped dependencies.</param>
    /// <param name="logger">Logger instance.</param>
    public CommandProcessingService(
        IServiceScopeFactory scopeFactory,
        ILogger<CommandProcessingService> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Command Processing Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingCommandsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing pending commands");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Command Processing Service stopped");
    }

    private async Task ProcessPendingCommandsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var commandRepository = scope.ServiceProvider.GetRequiredService<IRepository<HubCommand, int>>();
        var commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var pendingCommands = await commandRepository.FindAsync(
            c => c.Status == CommandStatus.Pending && c.SentAt <= DateTime.UtcNow,
            cancellationToken);

        if (!pendingCommands.Any())
        {
            return;
        }

        _logger.LogInformation("Processing {Count} pending commands", pendingCommands.Count);

        foreach (var command in pendingCommands)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                var dispatched = await commandDispatcher.DispatchCommandAsync(command, cancellationToken);

                if (dispatched)
                {
                    command.MarkSent();
                    _logger.LogInformation("Command {CommandId} sent successfully", command.Id);
                }
                else
                {
                    if (DateTime.UtcNow - command.SentAt > TimeSpan.FromMinutes(5))
                    {
                        command.MarkFailed("Device not connected within timeout period");
                        _logger.LogWarning("Command {CommandId} failed due to timeout", command.Id);
                    }
                }

                commandRepository.Update(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process command {CommandId}", command.Id);
                command.MarkFailed($"Exception: {ex.Message}");
                commandRepository.Update(command);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}