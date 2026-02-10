using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Team.Application.Commands.ApproveTimeOff;

/// <summary>
/// Command to approve a time off request.
/// </summary>
/// <param name="TimeOffId">The time off request ID.</param>
/// <param name="ApprovedByUserId">The ID of the user approving the request.</param>
public sealed record ApproveTimeOffCommand(
    Guid TimeOffId,
    Guid ApprovedByUserId) : ICommand;
