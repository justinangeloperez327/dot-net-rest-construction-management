using Construction.Application.Common.Errors;

namespace Construction.Application.Common.Results;

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    private Result(TValue value)
        : base(true, [])
    {
        _value = value;
    }

    private Result(IEnumerable<Error> errors)
        : base(false, errors)
    {
    }

    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failed result cannot be accessed.");

    public static Result<TValue> Success(TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(value);
    }

    public new static Result<TValue> Failure(params Error[] errors) => new(errors);
}
