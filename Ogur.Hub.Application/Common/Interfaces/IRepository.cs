// File: Ogur.Hub.Application/Common/Interfaces/IRepository.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Common.Interfaces

using System.Linq.Expressions;
using Ogur.Hub.Domain.Common;

namespace Ogur.Hub.Application.Common.Interfaces;

/// <summary>
/// Generic repository interface for data access.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TId">Entity ID type.</typeparam>
public interface IRepository<TEntity, TId> where TEntity : Entity<TId> where TId : IEquatable<TId>
{
    /// <summary>
    /// Gets an entity by ID.
    /// </summary>
    /// <param name="id">Entity ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Entity or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Finds entities matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of matching entities.</returns>
    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

    /// <summary>
    /// Gets a single entity matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Entity or null if not found.</returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Adds multiple entities.
    /// </summary>
    /// <param name="entities">Entities to add.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <param name="entity">Entity to remove.</param>
    void Remove(TEntity entity);
    
    /// <summary>
    /// Deletes an entity asynchronously.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Removes multiple entities.
    /// </summary>
    /// <param name="entities">Entities to remove.</param>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Checks if any entity matches a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if any entity matches.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

    /// <summary>
    /// Counts entities matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Count of matching entities.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    
    /// <summary>
    /// Finds entities matching a predicate with related data included.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <param name="includes">Related entities to include.</param>
    /// <returns>Collection of matching entities with includes.</returns>
    Task<IReadOnlyList<TEntity>> FindWithIncludesAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken ct = default,
        params string[] includes);
}