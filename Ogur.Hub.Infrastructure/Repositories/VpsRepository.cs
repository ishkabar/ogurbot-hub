// File: Ogur.Hub.Infrastructure/Repositories/VpsRepository.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Repositories

using Ogur.Hub.Application.Interfaces;
using Ogur.Hub.Domain.Entities;
using Ogur.Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ogur.Hub.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for VPS-related entities.
/// </summary>
public class VpsRepository : IVpsRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="VpsRepository"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public VpsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<VpsContainer>> GetAllContainersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VpsContainers.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<VpsContainer?> GetContainerByDockerIdAsync(string containerId, CancellationToken cancellationToken = default)
    {
        return await _context.VpsContainers
            .FirstOrDefaultAsync(c => c.ContainerId == containerId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddContainerAsync(VpsContainer container, CancellationToken cancellationToken = default)
    {
        await _context.VpsContainers.AddAsync(container, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateContainerAsync(VpsContainer container, CancellationToken cancellationToken = default)
    {
        _context.VpsContainers.Update(container);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<VpsWebsite>> GetAllWebsitesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VpsWebsites
            .Include(w => w.Container)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<VpsWebsite?> GetWebsiteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.VpsWebsites
            .Include(w => w.Container)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddWebsiteAsync(VpsWebsite website, CancellationToken cancellationToken = default)
    {
        await _context.VpsWebsites.AddAsync(website, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateWebsiteAsync(VpsWebsite website, CancellationToken cancellationToken = default)
    {
        _context.VpsWebsites.Update(website);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteWebsiteAsync(VpsWebsite website, CancellationToken cancellationToken = default)
    {
        _context.VpsWebsites.Remove(website);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<VpsResourceSnapshot>> GetAllSnapshotsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VpsResourceSnapshots.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddSnapshotAsync(VpsResourceSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await _context.VpsResourceSnapshots.AddAsync(snapshot, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<VpsWebsite?> GetWebsiteByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await _context.VpsWebsites
            .FirstOrDefaultAsync(w => w.Domain == domain, cancellationToken);
    }
    
    public async Task DeleteContainerAsync(VpsContainer container, CancellationToken cancellationToken = default)
    {
        _context.VpsContainers.Remove(container);
        await _context.SaveChangesAsync(cancellationToken);
    }
}