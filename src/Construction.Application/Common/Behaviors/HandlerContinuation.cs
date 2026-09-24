namespace Construction.Application.Common.Behaviors;

public delegate Task<TResponse> HandlerContinuation<TResponse>(
    CancellationToken cancellationToken);
