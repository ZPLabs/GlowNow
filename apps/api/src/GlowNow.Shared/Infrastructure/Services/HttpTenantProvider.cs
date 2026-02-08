using GlowNow.Shared.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GlowNow.Shared.Infrastructure.Services;

public sealed class HttpTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string BusinessIdHeader = "X-Business-Id";

    public HttpTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetCurrentBusinessId()
    {
        return TryGetCurrentBusinessId() 
               ?? throw new InvalidOperationException("Business ID header is missing or invalid.");
    }

    public Guid? TryGetCurrentBusinessId()
    {
        string? headerValue = _httpContextAccessor.HttpContext?.Request.Headers[BusinessIdHeader];

        if (Guid.TryParse(headerValue, out Guid businessId))
        {
            return businessId;
        }

        return null;
    }
}
