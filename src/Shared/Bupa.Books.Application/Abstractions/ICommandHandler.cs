namespace Bupa.Books.Application.Abstractions;

/// <summary>
/// Base interface for command handlers following CQRS pattern
/// </summary>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Marker interface for commands
/// </summary>
public interface ICommand<out TResult>
{
}

