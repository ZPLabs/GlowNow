namespace GlowNow.Infrastructure.Core.Application.Messaging;

/// <summary>
/// Represents a query that returns a <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response value.</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
