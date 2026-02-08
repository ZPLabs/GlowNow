namespace GlowNow.Shared.Application.Interfaces;

public interface ICurrentUserProvider
{
    Guid? UserId { get; }
    string? CognitoUserId { get; }
    void Set(Guid userId, string cognitoUserId);
}
