namespace GlowNow.Shared.Application.Messaging;

/// <summary>
/// Represents a handler for a command that returns a <see cref="Result"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

/// <summary>
/// Represents a handler for a command that returns a <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResponse">The type of the response value.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
