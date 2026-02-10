using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Team.Application.Commands.DeleteBlockedTime;

/// <summary>
/// Command to delete a blocked time.
/// </summary>
/// <param name="Id">The blocked time ID.</param>
public sealed record DeleteBlockedTimeCommand(Guid Id) : ICommand;
