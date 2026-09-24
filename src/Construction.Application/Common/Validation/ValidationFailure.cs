namespace Construction.Application.Common.Validation;

public sealed record ValidationFailure(
    string PropertyName,
    string ErrorCode,
    string Message);
