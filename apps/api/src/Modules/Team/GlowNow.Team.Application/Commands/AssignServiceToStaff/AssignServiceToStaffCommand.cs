using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Commands.AssignServiceToStaff;

/// <summary>
/// Command to assign a service to a staff member.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="ServiceId">The service ID to assign.</param>
public sealed record AssignServiceToStaffCommand(
    Guid StaffProfileId,
    Guid ServiceId) : ICommand;
