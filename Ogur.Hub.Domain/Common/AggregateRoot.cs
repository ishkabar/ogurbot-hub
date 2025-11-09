// File: Ogur.Hub.Domain/Common/AggregateRoot.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Common

namespace Ogur.Hub.Domain.Common;

/// <summary>
/// Base class for aggregate root entities in domain-driven design.
/// </summary>
/// <typeparam name="TId">Type of the aggregate root identifier.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId> where TId : IEquatable<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the collection of domain events raised by this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the aggregate.
    /// </summary>
    /// <param name="domainEvent">Domain event to add.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}