using Construction.Application.Common.Errors;
using Construction.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.IsSuccess
            ? new NoContentResult()
            : CreateFailureResult(result.Errors);
    }

    public static IActionResult ToActionResult<TValue>(this Result<TValue> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : CreateFailureResult(result.Errors);
    }

    private static IActionResult CreateFailureResult(
        IReadOnlyCollection<ApplicationError> errors)
    {
        ApplicationError primaryError = errors.First();

        int statusCode = primaryError.Type switch
        {
            ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = statusCode,
            Title = primaryError.Description
        };

        problemDetails.Extensions["errors"] = errors.Select(error => new
        {
            error.Code,
            error.Description,
            Type = error.Type.ToString()
        });

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}
