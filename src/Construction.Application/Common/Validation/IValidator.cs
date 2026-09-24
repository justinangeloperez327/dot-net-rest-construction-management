namespace Construction.Application.Common.Validation;

public interface IValidator<in TRequest>
{
    ValueTask<IReadOnlyCollection<ValidationFailure>> ValidateAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
