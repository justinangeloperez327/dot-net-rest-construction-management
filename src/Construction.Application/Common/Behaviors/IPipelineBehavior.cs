namespace Construction.Application.Common.Behaviors;

public interface IPipelineBehavior<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request,
        HandlerContinuation<TResponse> continuation,
        CancellationToken cancellationToken = default);
}
