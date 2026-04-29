namespace Bupa.Books.Domain.Entities;

/// <summary>
/// Base class for aggregate roots following Domain-Driven Design principles
/// </summary>
public abstract class AggregateRoot
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

