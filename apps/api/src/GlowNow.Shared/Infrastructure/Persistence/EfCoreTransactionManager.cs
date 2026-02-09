using System.Data;
using GlowNow.Shared.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace GlowNow.Shared.Infrastructure.Persistence;

public sealed class EfCoreTransactionManager : ITransactionManager
{
    private readonly AppDbContext _context;

    public EfCoreTransactionManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        if (_context.Database.CurrentTransaction is not null)
        {
            return await action(cancellationToken);
        }

        IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async ct =>
        {
            await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            try
            {
                T result = await action(ct);

                await transaction.CommitAsync(ct);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }
}
