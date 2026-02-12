namespace GlowNow.Team.Domain.Enums;

/// <summary>
/// Represents the status of a time off request.
/// </summary>
public enum TimeOffStatus
{
    /// <summary>
    /// Request is pending approval.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Request has been approved.
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Request has been rejected.
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Request has been cancelled.
    /// </summary>
    Cancelled = 4
}
