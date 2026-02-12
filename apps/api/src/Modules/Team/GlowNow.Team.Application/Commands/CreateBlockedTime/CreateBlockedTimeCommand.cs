using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Commands.CreateBlockedTime;

/// <summary>
/// Command to create a blocked time for a staff member.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="Title">Optional title/reason for the blocked time.</param>
/// <param name="StartTime">The start time.</param>
/// <param name="EndTime">The end time.</param>
/// <param name="IsRecurring">Whether this is a recurring blocked time.</param>
/// <param name="DayOfWeek">The day of week for recurring blocked time.</param>
/// <param name="SpecificDate">The specific date for one-time blocked time.</param>
public sealed record CreateBlockedTimeCommand(
    Guid StaffProfileId,
    string? Title,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsRecurring,
    DayOfWeek? DayOfWeek,
    DateOnly? SpecificDate) : ICommand<CreateBlockedTimeResponse>;

/// <summary>
/// Response DTO for CreateBlockedTimeCommand.
/// </summary>
/// <param name="Id">The created blocked time ID.</param>
public sealed record CreateBlockedTimeResponse(Guid Id);
