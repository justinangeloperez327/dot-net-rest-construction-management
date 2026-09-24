using Construction.Application.Common.Errors;

namespace Construction.Application.Common.Results;

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue value)
        : base(true, [])
    {
        _value = value;
    }

    internal Result(IEnumerable<ApplicationError> errors)
        : base(false, errors)
    {
    }

    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failed result cannot be accessed.");
}
