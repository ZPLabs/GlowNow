namespace GlowNow.Identity.Application.Commands.Login;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UserResponse User);

public sealed record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IEnumerable<MembershipResponse> Memberships);

public sealed record MembershipResponse(
    Guid BusinessId,
    UserRole Role);
