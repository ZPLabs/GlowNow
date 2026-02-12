using GlowNow.SharedKernel.Domain.Events;

namespace GlowNow.Team.Domain.Events;

/// <summary>
/// Domain event raised when a time off request is approved.
/// </summary>
/// <param name="TimeOffId">The time off request ID.</param>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="BusinessId">The business ID.</param>
/// <param name="ApprovedByUserId">The ID of the user who approved the request.</param>
public sealed record TimeOffApprovedEvent(
    Guid TimeOffId,
    Guid StaffProfileId,
    Guid BusinessId,
    Guid ApprovedByUserId) : IDomainEvent;
