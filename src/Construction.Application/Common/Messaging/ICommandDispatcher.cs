using Construction.Application.Common.Results;

namespace Construction.Application.Common.Messaging;

public interface ICommandDispatcher
{
    Task<Result> SendAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    Task<Result<TResponse>> SendAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>;
}
