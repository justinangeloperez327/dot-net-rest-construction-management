using Construction.Application.Common.Errors;

namespace Construction.Application.Common.Validation;

public static class ValidationErrorFactory
{
    public static Error[] Create(IEnumerable<ValidationFailure> failures)
    {
        ArgumentNullException.ThrowIfNull(failures);

        return failures
            .Select(failure => Error.Validation(
                failure.ErrorCode,
                string.IsNullOrWhiteSpace(failure.PropertyName)
                    ? failure.Message
                    : $"{failure.PropertyName}: {failure.Message}"))
            .ToArray();
    }
}
