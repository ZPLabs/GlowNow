using GlowNow.Identity.Application.Interfaces;
using GlowNow.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Identity.Infrastructure.Persistence.Repositories;

internal sealed class BusinessMembershipRepository : IBusinessMembershipRepository
{
    private readonly IdentityDbContext _context;

    public BusinessMembershipRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<BusinessMembership>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessMemberships
            .Where(m => m.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public void Add(BusinessMembership membership)
    {
        _context.BusinessMemberships.Add(membership);
    }
}
