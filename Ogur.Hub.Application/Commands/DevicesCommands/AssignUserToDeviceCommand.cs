// File: Ogur.Hub.Application/Commands/DevicesCommands/AssignUserToDeviceCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.DevicesCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Application.Commands.DevicesCommands;

/// <summary>
/// Command to assign a user to a device.
/// </summary>
/// <param name="DeviceId">Device identifier.</param>
/// <param name="UserId">User identifier.</param>
public sealed record AssignUserToDeviceCommand(int DeviceId, int UserId);

/// <summary>
/// Handler for AssignUserToDeviceCommand.
/// </summary>
public sealed class AssignUserToDeviceCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IRepository<Device, int> _deviceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignUserToDeviceCommandHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="deviceRepository">Device repository.</param>
    public AssignUserToDeviceCommandHandler(
        IApplicationDbContext context,
        IRepository<Device, int> deviceRepository)
    {
        _context = context;
        _deviceRepository = deviceRepository;
    }

    /// <summary>
    /// Handles the assign user to device command.
    /// </summary>
    /// <param name="command">Command to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if user was assigned, otherwise false.</returns>
    public async Task<bool> Handle(AssignUserToDeviceCommand command, CancellationToken ct)
    {
        var device = await _deviceRepository.GetByIdAsync(command.DeviceId, ct);
        if (device is null)
            return false;

        device.AssignUser(command.UserId);

        _deviceRepository.Update(device);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}