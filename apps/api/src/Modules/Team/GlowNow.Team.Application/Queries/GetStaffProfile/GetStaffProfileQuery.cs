using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Queries.GetAllStaff;

namespace GlowNow.Team.Application.Queries.GetStaffProfile;

/// <summary>
/// Query to get a specific staff profile by ID.
/// </summary>
/// <param name="Id">The staff profile ID.</param>
public sealed record GetStaffProfileQuery(Guid Id) : IQuery<StaffProfileResponse>;
