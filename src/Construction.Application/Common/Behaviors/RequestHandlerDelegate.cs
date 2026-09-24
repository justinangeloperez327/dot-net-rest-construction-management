namespace Construction.Application.Common.Behaviors;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(
    CancellationToken cancellationToken);
