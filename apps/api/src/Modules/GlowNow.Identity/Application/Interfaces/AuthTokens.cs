namespace GlowNow.Identity.Application.Interfaces;

public sealed record AuthTokens(
    string AccessToken,
    string IdToken,
    string RefreshToken,
    int ExpiresIn);
