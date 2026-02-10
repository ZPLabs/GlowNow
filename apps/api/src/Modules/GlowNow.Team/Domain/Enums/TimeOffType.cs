namespace GlowNow.Team.Domain.Enums;

/// <summary>
/// Represents the type of time off request.
/// </summary>
public enum TimeOffType
{
    /// <summary>
    /// Vacation time off.
    /// </summary>
    Vacation = 1,

    /// <summary>
    /// Sick leave.
    /// </summary>
    SickLeave = 2,

    /// <summary>
    /// Personal day off.
    /// </summary>
    PersonalDay = 3,

    /// <summary>
    /// Holiday time off.
    /// </summary>
    Holiday = 4,

    /// <summary>
    /// Training or professional development.
    /// </summary>
    Training = 5,

    /// <summary>
    /// Other type of time off.
    /// </summary>
    Other = 99
}
