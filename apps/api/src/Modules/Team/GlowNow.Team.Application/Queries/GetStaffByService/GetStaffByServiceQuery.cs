using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Queries.GetAllStaff;

namespace GlowNow.Team.Application.Queries.GetStaffByService;

/// <summary>
/// Query to get all staff profiles that can perform a specific service.
/// </summary>
/// <param name="ServiceId">The service ID.</param>
public sealed record GetStaffByServiceQuery(Guid ServiceId) : IQuery<IReadOnlyList<StaffProfileResponse>>;
