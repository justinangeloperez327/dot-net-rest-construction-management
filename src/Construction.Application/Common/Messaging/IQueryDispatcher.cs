using Construction.Application.Common.Results;

namespace Construction.Application.Common.Messaging;

public interface IQueryDispatcher
{
    Task<Result<TResponse>> SendAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>;
}
