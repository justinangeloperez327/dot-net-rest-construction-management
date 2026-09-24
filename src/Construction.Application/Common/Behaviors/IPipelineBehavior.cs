namespace Construction.Application.Common.Behaviors;

public interface IPipelineBehavior<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default);
}
