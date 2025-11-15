// File: Ogur.Hub.Application/Commands/DevicesCommands/UpdateDeviceCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.DevicesCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Application.Commands.DevicesCommands;

/// <summary>
/// Command to update device information.
/// </summary>
/// <param name="DeviceId">Device identifier.</param>
/// <param name="DeviceName">New device name.</param>
/// <param name="Description">New description.</param>
/// <param name="PrimaryUserId">Primary user identifier.</param>
public sealed record UpdateDeviceCommand(
    int DeviceId,
    string? DeviceName,
    string? Description,
    int? PrimaryUserId);

/// <summary>
/// Handler for UpdateDeviceCommand.
/// </summary>
public sealed class UpdateDeviceCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IRepository<Device, int> _deviceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDeviceCommandHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="deviceRepository">Device repository.</param>
    public UpdateDeviceCommandHandler(
        IApplicationDbContext context,
        IRepository<Device, int> deviceRepository)
    {
        _context = context;
        _deviceRepository = deviceRepository;
    }

    /// <summary>
    /// Handles the update device command.
    /// </summary>
    /// <param name="command">Command to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if device was updated, otherwise false.</returns>
    public async Task<bool> Handle(UpdateDeviceCommand command, CancellationToken ct)
    {
        var device = await _deviceRepository.GetByIdAsync(command.DeviceId, ct);
        if (device is null)
            return false;

        if (command.DeviceName != null)
            device.UpdateDeviceName(command.DeviceName);

        if (command.Description != null)
            device.UpdateDescription(command.Description);

        if (command.PrimaryUserId.HasValue)
            device.SetPrimaryUser(command.PrimaryUserId.Value);

        _deviceRepository.Update(device);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}