// File: Ogur.Hub.Application/Commands/DevicesCommands/BlockDeviceCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.DevicesCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using MediatR;

namespace Ogur.Hub.Application.Commands.DevicesCommands;

/// <summary>
/// Command to block a device from accessing the system.
/// </summary>
/// <param name="DeviceId">Device identifier to block.</param>
public sealed record BlockDeviceCommand(int DeviceId) : IRequest<bool>;

/// <summary>
/// Handler for blocking a device.
/// </summary>
public sealed class BlockDeviceCommandHandler : IRequestHandler<BlockDeviceCommand, bool>
{
    private readonly IRepository<Device, int> _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlockDeviceCommandHandler"/> class.
    /// </summary>
    /// <param name="deviceRepository">Device repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public BlockDeviceCommandHandler(
        IRepository<Device, int> deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public async Task<bool> Handle(BlockDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
        {
            return false;
        }

        device.Block();
        _deviceRepository.Update(device);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}