// File: Hub.Application/Commands/LicensesCommands/DeleteLicenseCommand.cs
// Project: Hub.Application
// Namespace: Ogur.Hub.Application.Commands.LicensesCommands

using MediatR;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Application.Commands.LicensesCommands;

/// <summary>
/// Command to delete a license
/// </summary>
public sealed record DeleteLicenseCommand(int LicenseId) : IRequest<bool>;

/// <summary>
/// Handler for DeleteLicenseCommand
/// </summary>
public sealed class DeleteLicenseCommandHandler : IRequestHandler<DeleteLicenseCommand, bool>
{
    private readonly IRepository<License, int> _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes handler
    /// </summary>
    public DeleteLicenseCommandHandler(
        IRepository<License, int> licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the delete license command
    /// </summary>
    public async Task<bool> Handle(DeleteLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await _licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);

        if (license is null)
        {
            return false;
        }

        await _licenseRepository.DeleteAsync(license, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}