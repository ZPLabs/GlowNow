using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Business.Application.Queries.GetBusinessDetails;

/// <summary>
/// Query to get the full details of a business.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
public sealed record GetBusinessDetailsQuery(Guid BusinessId) : IQuery<BusinessDetailsResponse>;
