using Construction.Application.Common.Errors;

namespace Construction.Application.Common.Results;

public interface IResult
{
    bool IsSuccess { get; }

    bool IsFailure { get; }

    IReadOnlyCollection<Error> Errors { get; }
}
