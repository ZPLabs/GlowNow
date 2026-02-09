using GlowNow.Shared.Application.Interfaces;

namespace GlowNow.Shared.Infrastructure.Services;

internal sealed class CurrentUserProvider : ICurrentUserProvider
{
    public Guid? UserId { get; private set; }
    public string? CognitoUserId { get; private set; }

    public void Set(Guid userId, string cognitoUserId)
    {
        if (UserId.HasValue)
        {
            throw new InvalidOperationException("User context is already set.");
        }

        UserId = userId;
        CognitoUserId = cognitoUserId;
    }
}
