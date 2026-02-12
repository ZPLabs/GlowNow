using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Commands.RequestTimeOff;

/// <summary>
/// Command to request time off for a staff member.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="StartDate">The start date of the time off.</param>
/// <param name="EndDate">The end date of the time off (inclusive).</param>
/// <param name="Type">The type of time off.</param>
/// <param name="Notes">Optional notes for the request.</param>
public sealed record RequestTimeOffCommand(
    Guid StaffProfileId,
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOffType Type,
    string? Notes) : ICommand<RequestTimeOffResponse>;

/// <summary>
/// Response DTO for RequestTimeOffCommand.
/// </summary>
/// <param name="Id">The created time off request ID.</param>
public sealed record RequestTimeOffResponse(Guid Id);
