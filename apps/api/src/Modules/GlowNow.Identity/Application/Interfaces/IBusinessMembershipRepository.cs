namespace GlowNow.Identity.Application.Interfaces;

public interface IBusinessMembershipRepository
{
    Task<List<BusinessMembership>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    void Add(BusinessMembership membership);
}
