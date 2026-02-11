using GlowNow.Business.Application.Queries.GetBusinessDetails;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Business.Application.Queries.GetOperatingHours;

/// <summary>
/// Query to get the operating hours for a business.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
public sealed record GetOperatingHoursQuery(Guid BusinessId) : IQuery<OperatingHoursResponse>;
