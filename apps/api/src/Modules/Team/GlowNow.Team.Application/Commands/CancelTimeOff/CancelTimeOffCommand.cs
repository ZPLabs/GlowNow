using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Commands.CancelTimeOff;

/// <summary>
/// Command to cancel a time off request.
/// </summary>
/// <param name="TimeOffId">The time off request ID.</param>
public sealed record CancelTimeOffCommand(Guid TimeOffId) : ICommand;
