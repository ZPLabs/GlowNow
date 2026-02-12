namespace GlowNow.SharedKernel.Domain.Primitives;

/// <summary>
/// Represents a domain entity with a unique identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// Required for EF Core materialization.
    /// </summary>
    protected Entity()
    {
        Id = default!;
    }

    public TId Id { get; protected init; }

    public static bool operator ==(Entity<TId>? first, Entity<TId>? second)
    {
        return first is not null && second is not null && first.Equals(second);
    }

    public static bool operator !=(Entity<TId>? first, Entity<TId>? second)
    {
        return !(first == second);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj.GetType() != GetType()) return false;
        if (obj is not Entity<TId> entity) return false;

        return entity.Id.Equals(Id);
    }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;

        return other.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() * 41;
    }
}
