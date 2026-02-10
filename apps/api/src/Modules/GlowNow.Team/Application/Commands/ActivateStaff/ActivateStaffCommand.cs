using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Team.Application.Commands.ActivateStaff;

/// <summary>
/// Command to activate a staff member.
/// </summary>
/// <param name="Id">The staff profile ID.</param>
public sealed record ActivateStaffCommand(Guid Id) : ICommand;
