namespace GlowNow.Shared.Application.Interfaces;

/// <summary>
/// Defines a contract for the unit of work pattern.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
