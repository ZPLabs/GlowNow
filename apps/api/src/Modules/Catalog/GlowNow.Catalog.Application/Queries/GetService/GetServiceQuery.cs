using GlowNow.Catalog.Application.Queries.GetAllServices;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Catalog.Application.Queries.GetService;

/// <summary>
/// Query to get a specific service by ID.
/// </summary>
/// <param name="Id">The service ID.</param>
public sealed record GetServiceQuery(Guid Id) : IQuery<ServiceResponse>;
