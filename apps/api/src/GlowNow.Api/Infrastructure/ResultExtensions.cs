using System.Diagnostics;
using GlowNow.Shared.Domain.Errors;
using GlowNow.Shared.Domain.Primitives;

namespace GlowNow.Api.Infrastructure;

public static class ResultExtensions
{
    public static IResult ToApiResponse(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(new { Meta = CreateMeta() });
        }

        return MapError(result.Error);
    }

    public static IResult ToApiResponse<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(new ApiResponse<T>(result.Value, CreateMeta()));
        }

        return MapError(result.Error);
    }

    private static IResult MapError(Error error)
    {
        var apiError = new ApiError(error.Code, error.Message);

        if (error is ValidationError validationError)
        {
            var details = validationError.Errors.Select(e => new ApiErrorDetail(e.Code, e.Message));
            apiError = apiError with { Details = details };
            return Results.BadRequest(new ApiErrorResponse(apiError));
        }

        return error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new ApiErrorResponse(apiError)),
            ErrorType.Conflict => Results.Conflict(new ApiErrorResponse(apiError)),
            ErrorType.Validation => Results.BadRequest(new ApiErrorResponse(apiError)),
            _ => Results.InternalServerError(new ApiErrorResponse(apiError))
        };
    }

    private static ApiMeta CreateMeta() => new(DateTime.UtcNow, Activity.Current?.Id ?? Guid.NewGuid().ToString());
}
