using GlowNow.Identity.Application.Interfaces;
using GlowNow.Shared.Infrastructure.Persistence;

namespace GlowNow.Identity.Infrastructure.Persistence.Repositories;

internal sealed class BusinessMembershipRepository : IBusinessMembershipRepository
{
    private readonly AppDbContext _context;

    public BusinessMembershipRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BusinessMembership>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<BusinessMembership>()
            .Where(m => m.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public void Add(BusinessMembership membership)
    {
        _context.Set<BusinessMembership>().Add(membership);
    }
}
