// File: Ogur.Hub.Application/Commands/TelemetryCommands/ReceiveTelemetryCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.TelemetryCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using MediatR;

namespace Ogur.Hub.Application.Commands.TelemetryCommands;

/// <summary>
/// Command to receive and store telemetry data from a device.
/// </summary>
/// <param name="DeviceId">Device identifier sending telemetry.</param>
/// <param name="EventType">Type of telemetry event.</param>
/// <param name="EventDataJson">Event data as JSON string.</param>
/// <param name="OccurredAt">When the event occurred on client side.</param>
public sealed record ReceiveTelemetryCommand(
    int DeviceId,
    string EventType,
    string? EventDataJson,
    DateTime OccurredAt) : IRequest<bool>;

/// <summary>
/// Handler for receiving telemetry from devices.
/// </summary>
public sealed class ReceiveTelemetryCommandHandler : IRequestHandler<ReceiveTelemetryCommand, bool>
{
    private readonly IRepository<Device, int> _deviceRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceiveTelemetryCommandHandler"/> class.
    /// </summary>
    /// <param name="deviceRepository">Device repository.</param>
    /// <param name="context">Database context.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public ReceiveTelemetryCommandHandler(
        IRepository<Device, int> deviceRepository,
        IApplicationDbContext context,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public async Task<bool> Handle(ReceiveTelemetryCommand request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
        {
            return false;
        }

        var telemetry = Telemetry.Create(
            deviceId: request.DeviceId,
            eventType: request.EventType,
            eventDataJson: request.EventDataJson,
            occurredAt: request.OccurredAt);

        _context.Telemetry.Add(telemetry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}