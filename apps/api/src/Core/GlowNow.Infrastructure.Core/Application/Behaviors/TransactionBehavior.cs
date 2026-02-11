namespace GlowNow.Infrastructure.Core.Application.Behaviors;

/// <summary>
/// Pipeline behavior that wraps commands in a database transaction.
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ITransactionManager _transactionManager;

    public TransactionBehavior(ITransactionManager transactionManager)
    {
        _transactionManager = transactionManager;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IBaseCommand)
        {
            return await next();
        }

        return await _transactionManager.ExecuteInTransactionAsync(
            async ct => await next(),
            cancellationToken);
    }
}
