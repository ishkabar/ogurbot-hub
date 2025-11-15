// File: Ogur.Hub.Application/Commands/DevicesCommands/UnblockDeviceCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.DevicesCommands

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Application.Commands.DevicesCommands;

/// <summary>
/// Command to unblock a device.
/// </summary>
/// <param name="DeviceId">Device identifier.</param>
public sealed record UnblockDeviceCommand(int DeviceId) : IRequest<bool>;


/// <summary>
/// Handler for unblocking a device.
/// </summary>
public sealed class UnblockDeviceCommandHandler : IRequestHandler<UnblockDeviceCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UnblockDeviceCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnblockDeviceCommandHandler"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="logger">Logger instance.</param>
    public UnblockDeviceCommandHandler(
        IApplicationDbContext context,
        ILogger<UnblockDeviceCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> Handle(UnblockDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await _context.Devices
            .FirstOrDefaultAsync(d => d.Id == request.DeviceId, cancellationToken);

        if (device == null)
        {
            _logger.LogWarning("Device {DeviceId} not found for unblocking", request.DeviceId);
            return false;
        }

        device.Unblock();
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Device {DeviceId} ({DeviceName}) has been unblocked", 
            device.Id, device.DeviceName);

        return true;
    }
}