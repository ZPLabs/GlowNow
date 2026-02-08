namespace GlowNow.Shared.Application.Interfaces;

/// <summary>
/// A no-op transaction manager that simply executes the action.
/// </summary>
internal sealed class NoOpTransactionManager : ITransactionManager
{
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        return await action(cancellationToken);
    }
}
