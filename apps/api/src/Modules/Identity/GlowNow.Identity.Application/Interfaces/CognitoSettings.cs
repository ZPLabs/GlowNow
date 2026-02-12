namespace GlowNow.Identity.Application.Interfaces;

public sealed class CognitoSettings
{
    public const string SectionName = "Cognito";

    public string UserPoolId { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string? AccessKey { get; init; }
    public string? SecretKey { get; init; }
}
