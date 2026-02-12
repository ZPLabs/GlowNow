using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Commands.UpdateStaffProfile;

/// <summary>
/// Command to update a staff profile.
/// </summary>
/// <param name="Id">The staff profile ID.</param>
/// <param name="Title">The staff member's title/role.</param>
/// <param name="Bio">The optional bio/description.</param>
/// <param name="ProfileImageUrl">The optional profile image URL.</param>
/// <param name="DisplayOrder">The display order for sorting.</param>
/// <param name="AcceptsOnlineBookings">Whether the staff accepts online bookings.</param>
public sealed record UpdateStaffProfileCommand(
    Guid Id,
    string Title,
    string? Bio,
    string? ProfileImageUrl,
    int DisplayOrder,
    bool AcceptsOnlineBookings) : ICommand;
