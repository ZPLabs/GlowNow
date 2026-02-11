using System.Diagnostics;
using GlowNow.SharedKernel.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace GlowNow.Identity.Api.Infrastructure;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(new { Meta = CreateMeta() });
        }

        return MapError(result.Error);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(new ApiResponse<T>(result.Value, CreateMeta()));
        }

        return MapError(result.Error);
    }

    private static IActionResult MapError(Error error)
    {
        var apiError = new ApiError(error.Code, error.Message);

        if (error is ValidationError validationError)
        {
            var details = validationError.Errors.Select(e => new ApiErrorDetail(e.Code, e.Message));
            apiError = apiError with { Details = details };
            return new BadRequestObjectResult(new ApiErrorResponse(apiError));
        }

        return error.Type switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(new ApiErrorResponse(apiError)),
            ErrorType.Conflict => new ConflictObjectResult(new ApiErrorResponse(apiError)),
            ErrorType.Validation => new BadRequestObjectResult(new ApiErrorResponse(apiError)),
            _ => new ObjectResult(new ApiErrorResponse(apiError)) { StatusCode = 500 }
        };
    }

    private static ApiMeta CreateMeta() => new(DateTime.UtcNow, Activity.Current?.Id ?? Guid.NewGuid().ToString());
}
