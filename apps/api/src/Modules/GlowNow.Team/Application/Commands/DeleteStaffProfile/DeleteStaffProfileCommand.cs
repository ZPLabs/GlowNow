using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Team.Application.Commands.DeleteStaffProfile;

/// <summary>
/// Command to soft-delete a staff profile.
/// </summary>
/// <param name="Id">The staff profile ID.</param>
public sealed record DeleteStaffProfileCommand(Guid Id) : ICommand;
