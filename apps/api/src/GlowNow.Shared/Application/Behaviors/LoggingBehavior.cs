namespace GlowNow.Shared.Application.Behaviors;

/// <summary>
/// Logs the request name and payload on entry, and the request name on completion.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting request {@RequestName}, {@DateTimeUtc}", typeof(TRequest).Name, DateTime.UtcNow);

        var response = await next();

        if (response.IsFailure)
        {
            _logger.LogError(
                "Request {@RequestName} failed with error {@Error}, {@DateTimeUtc}",
                typeof(TRequest).Name,
                response.Error,
                DateTime.UtcNow);
        }

        _logger.LogInformation("Completed request {@RequestName}, {@DateTimeUtc}", typeof(TRequest).Name, DateTime.UtcNow);

        return response;
    }
}
