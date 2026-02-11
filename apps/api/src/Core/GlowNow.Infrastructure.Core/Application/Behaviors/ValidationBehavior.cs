namespace GlowNow.Infrastructure.Core.Application.Behaviors;

/// <summary>
/// Pipeline behavior that runs validation for all requests.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationFailures
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .Select(failure => new Error(
                failure.ErrorCode,
                failure.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Length != 0)
        {
            return CreateValidationResult(errors);
        }

        return await next();
    }

    private static TResponse CreateValidationResult(Error[] errors)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (Result.Failure(ValidationError.FromErrors(errors)) as TResponse)!;
        }

        object validationResult = typeof(Result<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(TResponse).GetGenericArguments()[0])
            .GetMethod(nameof(Result.Failure))!
            .Invoke(null, [ValidationError.FromErrors(errors)])!;

        return (TResponse)validationResult;
    }
}
