using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Team.Application.Commands.CreateStaffProfile;

/// <summary>
/// Command to create a new staff profile.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
/// <param name="UserId">The user ID to create a staff profile for.</param>
/// <param name="Title">The staff member's title/role.</param>
/// <param name="Bio">The optional bio/description.</param>
/// <param name="ProfileImageUrl">The optional profile image URL.</param>
/// <param name="DisplayOrder">The display order for sorting.</param>
/// <param name="AcceptsOnlineBookings">Whether the staff accepts online bookings.</param>
public sealed record CreateStaffProfileCommand(
    Guid BusinessId,
    Guid UserId,
    string Title,
    string? Bio,
    string? ProfileImageUrl,
    int DisplayOrder,
    bool AcceptsOnlineBookings) : ICommand<CreateStaffProfileResponse>;

/// <summary>
/// Response DTO for CreateStaffProfileCommand.
/// </summary>
/// <param name="Id">The created staff profile ID.</param>
public sealed record CreateStaffProfileResponse(Guid Id);
