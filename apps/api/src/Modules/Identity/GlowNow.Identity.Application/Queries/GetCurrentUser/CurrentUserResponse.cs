using GlowNow.Identity.Domain.Enums;

namespace GlowNow.Identity.Application.Queries.GetCurrentUser;

public sealed record CurrentUserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    IEnumerable<UserMembershipResponse> Memberships);

public sealed record UserMembershipResponse(
    Guid BusinessId,
    string BusinessName,
    UserRole Role);
