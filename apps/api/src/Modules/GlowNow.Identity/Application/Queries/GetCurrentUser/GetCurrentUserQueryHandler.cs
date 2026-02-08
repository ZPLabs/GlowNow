using GlowNow.Identity.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Identity.Application.Queries.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, CurrentUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, IBusinessRepository businessRepository)
    {
        _userRepository = userRepository;
        _businessRepository = businessRepository;
    }

    public async Task<Result<CurrentUserResponse>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<CurrentUserResponse>(IdentityErrors.UserNotFound);
        }

        var memberships = new List<UserMembershipResponse>();

        foreach (var membership in user.Memberships)
        {
            var business = await _businessRepository.GetByIdAsync(membership.BusinessId, cancellationToken);
            memberships.Add(new UserMembershipResponse(
                membership.BusinessId,
                business?.Name ?? "Unknown Business",
                membership.Role));
        }

        return new CurrentUserResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber?.Value,
            memberships);
    }
}
