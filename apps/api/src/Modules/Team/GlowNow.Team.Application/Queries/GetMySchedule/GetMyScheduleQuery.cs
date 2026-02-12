using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Queries.GetStaffSchedule;

namespace GlowNow.Team.Application.Queries.GetMySchedule;

/// <summary>
/// Query to get the current user's schedule.
/// </summary>
/// <param name="UserId">The current user's ID.</param>
/// <param name="BusinessId">The business ID.</param>
public sealed record GetMyScheduleQuery(
    Guid UserId,
    Guid BusinessId) : IQuery<StaffScheduleResponse>;
