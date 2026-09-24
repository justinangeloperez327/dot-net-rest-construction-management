using Construction.Application.Common.Errors;

namespace Construction.Application.Common.Results;

public class Result : IResult
{
    private readonly Error[] _errors;

    protected Result(bool isSuccess, IEnumerable<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        _errors = [.. errors];

        if (isSuccess && _errors.Length != 0)
        {
            throw new ArgumentException("A successful result cannot contain errors.", nameof(errors));
        }

        if (!isSuccess && _errors.Length == 0)
        {
            throw new ArgumentException("A failed result must contain at least one error.", nameof(errors));
        }

        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public IReadOnlyCollection<Error> Errors => _errors;

    public static Result Success() => new(true, []);

    public static Result Failure(params Error[] errors) => new(false, errors);
}
