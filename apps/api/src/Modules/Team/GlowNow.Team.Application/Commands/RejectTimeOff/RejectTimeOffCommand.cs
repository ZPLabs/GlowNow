using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Commands.RejectTimeOff;

/// <summary>
/// Command to reject a time off request.
/// </summary>
/// <param name="TimeOffId">The time off request ID.</param>
/// <param name="Reason">The optional reason for rejection.</param>
public sealed record RejectTimeOffCommand(
    Guid TimeOffId,
    string? Reason) : ICommand;
