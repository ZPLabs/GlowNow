namespace GlowNow.Team.Domain.Enums;

/// <summary>
/// Represents the status of a staff member.
/// </summary>
public enum StaffStatus
{
    /// <summary>
    /// Staff member is pending activation.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Staff member is active and can receive bookings.
    /// </summary>
    Active = 2,

    /// <summary>
    /// Staff member is temporarily on leave.
    /// </summary>
    OnLeave = 3,

    /// <summary>
    /// Staff member is inactive and cannot receive bookings.
    /// </summary>
    Inactive = 4
}
