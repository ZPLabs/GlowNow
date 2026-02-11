using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Team.Domain.Errors;

/// <summary>
/// Contains all team-related domain errors.
/// </summary>
public static class TeamErrors
{
    // Staff Profile Errors
    public static readonly Error StaffProfileNotFound = Error.NotFound(
        "Team.StaffProfileNotFound",
        "The staff profile with the specified identifier was not found.");

    public static readonly Error StaffAlreadyExists = Error.Conflict(
        "Team.StaffAlreadyExists",
        "A staff profile already exists for this user in this business.");

    public static readonly Error InvalidStaffTitle = Error.Validation(
        "Team.InvalidStaffTitle",
        "The staff title cannot be empty.");

    public static readonly Error StaffNotActive = Error.Conflict(
        "Team.StaffNotActive",
        "The staff member is not currently active.");

    public static readonly Error StaffAlreadyActive = Error.Conflict(
        "Team.StaffAlreadyActive",
        "The staff member is already active.");

    public static readonly Error StaffAlreadyInactive = Error.Conflict(
        "Team.StaffAlreadyInactive",
        "The staff member is already inactive.");

    public static readonly Error CannotDeleteActiveStaff = Error.Conflict(
        "Team.CannotDeleteActiveStaff",
        "Cannot delete an active staff member. Deactivate them first.");

    // Service Assignment Errors
    public static readonly Error ServiceAlreadyAssigned = Error.Conflict(
        "Team.ServiceAlreadyAssigned",
        "This service is already assigned to this staff member.");

    public static readonly Error ServiceNotAssigned = Error.NotFound(
        "Team.ServiceNotAssigned",
        "This service is not assigned to this staff member.");

    public static readonly Error ServiceNotFound = Error.NotFound(
        "Team.ServiceNotFound",
        "The service with the specified identifier was not found.");

    // Schedule Errors
    public static readonly Error InvalidWorkDayTimes = Error.Validation(
        "Team.InvalidWorkDayTimes",
        "The work day end time must be after the start time.");

    public static readonly Error InvalidBreakTimes = Error.Validation(
        "Team.InvalidBreakTimes",
        "The break end time must be after the break start time.");

    public static readonly Error BreakOutsideWorkHours = Error.Validation(
        "Team.BreakOutsideWorkHours",
        "The break times must be within work hours.");

    public static readonly Error InvalidWeeklySchedule = Error.Validation(
        "Team.InvalidWeeklySchedule",
        "The weekly schedule is invalid.");

    // Time Off Errors
    public static readonly Error TimeOffNotFound = Error.NotFound(
        "Team.TimeOffNotFound",
        "The time off request with the specified identifier was not found.");

    public static readonly Error InvalidTimeOffDates = Error.Validation(
        "Team.InvalidTimeOffDates",
        "The time off end date must be on or after the start date.");

    public static readonly Error TimeOffOverlap = Error.Conflict(
        "Team.TimeOffOverlap",
        "The requested time off overlaps with an existing approved time off period.");

    public static readonly Error TimeOffAlreadyApproved = Error.Conflict(
        "Team.TimeOffAlreadyApproved",
        "This time off request has already been approved.");

    public static readonly Error TimeOffAlreadyRejected = Error.Conflict(
        "Team.TimeOffAlreadyRejected",
        "This time off request has already been rejected.");

    public static readonly Error TimeOffAlreadyCancelled = Error.Conflict(
        "Team.TimeOffAlreadyCancelled",
        "This time off request has already been cancelled.");

    public static readonly Error TimeOffNotPending = Error.Conflict(
        "Team.TimeOffNotPending",
        "Only pending time off requests can be approved or rejected.");

    public static readonly Error CannotCancelPastTimeOff = Error.Conflict(
        "Team.CannotCancelPastTimeOff",
        "Cannot cancel time off that has already started.");

    // Blocked Time Errors
    public static readonly Error BlockedTimeNotFound = Error.NotFound(
        "Team.BlockedTimeNotFound",
        "The blocked time with the specified identifier was not found.");

    public static readonly Error InvalidBlockedTimeTimes = Error.Validation(
        "Team.InvalidBlockedTimeTimes",
        "The blocked time end must be after the start time.");

    public static readonly Error RecurringBlockedTimeMissingDay = Error.Validation(
        "Team.RecurringBlockedTimeMissingDay",
        "Recurring blocked time must specify a day of week.");

    public static readonly Error OneTimeBlockedTimeMissingDate = Error.Validation(
        "Team.OneTimeBlockedTimeMissingDate",
        "One-time blocked time must specify a specific date.");

    public static readonly Error BlockedTimeOverlap = Error.Conflict(
        "Team.BlockedTimeOverlap",
        "The blocked time overlaps with an existing blocked time period.");

    // User/Identity Errors
    public static readonly Error UserNotFound = Error.NotFound(
        "Team.UserNotFound",
        "The user with the specified identifier was not found.");

    public static readonly Error UserNotBusinessMember = Error.Validation(
        "Team.UserNotBusinessMember",
        "The user is not a member of this business.");
}
