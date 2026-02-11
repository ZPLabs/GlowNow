using System.Diagnostics;

namespace GlowNow.Infrastructure.Core.Application.Behaviors;

/// <summary>
/// Pipeline behavior that logs slow requests.
/// </summary>
public sealed class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;

    public PerformanceBehavior(ILogger<TRequest> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 1000)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(
                "GlowNow Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                requestName,
                elapsedMilliseconds,
                request);
        }
        else if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogWarning(
                "GlowNow Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                requestName,
                elapsedMilliseconds,
                request);
        }

        return response;
    }
}
