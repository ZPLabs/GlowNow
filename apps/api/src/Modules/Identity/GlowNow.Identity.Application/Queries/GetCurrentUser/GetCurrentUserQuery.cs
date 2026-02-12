using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Identity.Application.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IQuery<CurrentUserResponse>;
