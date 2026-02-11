namespace GlowNow.Identity.Api.Infrastructure;

public sealed record ApiResponse<T>(T Data, ApiMeta Meta);

public sealed record ApiMeta(DateTime Timestamp, string RequestId);

public sealed record ApiErrorResponse(ApiError Error);

public sealed record ApiError(string Code, string Message, IEnumerable<ApiErrorDetail>? Details = null);

public sealed record ApiErrorDetail(string Field, string Message);
