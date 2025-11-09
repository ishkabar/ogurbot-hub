// File: Ogur.Hub.Domain/Common/IDomainEvent.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Common

namespace Ogur.Hub.Domain.Common;

/// <summary>
/// Marker interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTime OccurredAt { get; }
}