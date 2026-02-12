namespace GlowNow.Infrastructure.Core.Application.Interfaces;

/// <summary>
/// Defines a contract for managing database transactions.
/// </summary>
public interface ITransactionManager
{
    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default);
}
