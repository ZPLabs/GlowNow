using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Team.Application.Commands.DeactivateStaff;

/// <summary>
/// Command to deactivate a staff member.
/// </summary>
/// <param name="Id">The staff profile ID.</param>
public sealed record DeactivateStaffCommand(Guid Id) : ICommand;
