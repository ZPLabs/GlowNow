using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Commands.UnassignServiceFromStaff;

/// <summary>
/// Command to unassign a service from a staff member.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="ServiceId">The service ID to unassign.</param>
public sealed record UnassignServiceFromStaffCommand(
    Guid StaffProfileId,
    Guid ServiceId) : ICommand;
