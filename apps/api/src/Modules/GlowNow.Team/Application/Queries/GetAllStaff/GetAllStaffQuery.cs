using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Team.Application.Queries.GetAllStaff;

/// <summary>
/// Query to get all staff profiles for a business.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
/// <param name="ActiveOnly">Whether to only return active staff.</param>
public sealed record GetAllStaffQuery(
    Guid BusinessId,
    bool ActiveOnly = false) : IQuery<IReadOnlyList<StaffProfileResponse>>;

/// <summary>
/// Response DTO for a staff profile.
/// </summary>
/// <param name="Id">The staff profile ID.</param>
/// <param name="UserId">The associated user ID.</param>
/// <param name="Title">The staff member's title/role.</param>
/// <param name="Bio">The staff member's bio/description.</param>
/// <param name="ProfileImageUrl">The profile image URL.</param>
/// <param name="DisplayOrder">The display order for sorting.</param>
/// <param name="AcceptsOnlineBookings">Whether the staff accepts online bookings.</param>
/// <param name="Status">The staff member's current status.</param>
/// <param name="ServiceIds">The IDs of services this staff can perform.</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the profile was created.</param>
public sealed record StaffProfileResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string? Bio,
    string? ProfileImageUrl,
    int DisplayOrder,
    bool AcceptsOnlineBookings,
    string Status,
    IReadOnlyList<Guid> ServiceIds,
    DateTime CreatedAtUtc)
{
    /// <summary>
    /// Creates a StaffProfileResponse from a StaffProfile entity.
    /// </summary>
    public static StaffProfileResponse FromEntity(StaffProfile entity)
    {
        return new StaffProfileResponse(
            entity.Id,
            entity.UserId,
            entity.Title,
            entity.Bio,
            entity.ProfileImageUrl,
            entity.DisplayOrder,
            entity.AcceptsOnlineBookings,
            entity.Status.ToString(),
            entity.ServiceAssignments.Select(sa => sa.ServiceId).ToList(),
            entity.CreatedAtUtc);
    }
}
