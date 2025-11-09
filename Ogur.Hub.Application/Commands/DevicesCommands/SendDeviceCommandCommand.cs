// File: Ogur.Hub.Application/Commands/DevicesCommands/SendDeviceCommandCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.DevicesCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using Ogur.Hub.Domain.Enums;
using MediatR;

namespace Ogur.Hub.Application.Commands.DevicesCommands;

/// <summary>
/// Command to send a hub command to a device.
/// </summary>
/// <param name="DeviceId">Target device identifier.</param>
/// <param name="CommandType">Type of command to send.</param>
/// <param name="Payload">Command payload as JSON string.</param>
public sealed record SendDeviceCommandCommand(
    int DeviceId,
    CommandType CommandType,
    string Payload) : IRequest<int>;

/// <summary>
/// Handler for sending commands to devices.
/// </summary>
public sealed class SendDeviceCommandCommandHandler : IRequestHandler<SendDeviceCommandCommand, int>
{
    private readonly IRepository<Device, int> _deviceRepository;
    private readonly IRepository<HubCommand, int> _commandRepository;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendDeviceCommandCommandHandler"/> class.
    /// </summary>
    /// <param name="deviceRepository">Device repository.</param>
    /// <param name="commandRepository">Hub command repository.</param>
    /// <param name="commandDispatcher">Command dispatcher.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public SendDeviceCommandCommandHandler(
        IRepository<Device, int> deviceRepository,
        IRepository<HubCommand, int> commandRepository,
        ICommandDispatcher commandDispatcher,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _commandRepository = commandRepository ?? throw new ArgumentNullException(nameof(commandRepository));
        _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public async Task<int> Handle(SendDeviceCommandCommand request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
        {
            throw new InvalidOperationException($"Device with ID {request.DeviceId} not found");
        }

        var command = HubCommand.Create(
            deviceId: request.DeviceId,
            commandType: request.CommandType,
            payload: request.Payload);

        await _commandRepository.AddAsync(command, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dispatched = await _commandDispatcher.DispatchCommandAsync(command, cancellationToken);

        if (dispatched)
        {
            command.MarkSent();
            _commandRepository.Update(command);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return command.Id;
    }
}