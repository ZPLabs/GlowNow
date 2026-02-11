namespace GlowNow.Infrastructure.Core.Application.Interfaces;

public interface ICurrentUserProvider
{
    Guid? UserId { get; }
    string? CognitoUserId { get; }
    void Set(Guid userId, string cognitoUserId);
}
