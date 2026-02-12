using GlowNow.Infrastructure.Core.Application.Interfaces;

namespace GlowNow.Api.Middleware;

public sealed class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private const string BusinessIdHeader = "X-Business-Id";

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserProvider currentUserProvider)
    {
        if (context.Request.Headers.TryGetValue(BusinessIdHeader, out var businessIdValues))
        {
            if (Guid.TryParse(businessIdValues.FirstOrDefault(), out Guid businessId))
            {
                // Here we could validate if the current user has access to this business
                // For now, we just let it pass if it's a valid GUID
                // Real implementation would check currentUserProvider.UserId memberships
            }
        }

        await _next(context);
    }
}
