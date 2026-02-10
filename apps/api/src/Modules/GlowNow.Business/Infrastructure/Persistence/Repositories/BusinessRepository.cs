using GlowNow.Business.Application.Interfaces;
using GlowNow.Business.Domain.ValueObjects;
using GlowNow.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BusinessEntity = GlowNow.Business.Domain.Entities.Business;

namespace GlowNow.Business.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Business aggregate.
/// </summary>
internal sealed class BusinessRepository : IBusinessRepository
{
    private readonly AppDbContext _context;

    public BusinessRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BusinessEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<BusinessEntity>()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByRucAsync(Ruc ruc, CancellationToken cancellationToken = default)
    {
        return await _context.Set<BusinessEntity>()
            .AnyAsync(b => b.Ruc.Value == ruc.Value, cancellationToken);
    }

    public void Add(BusinessEntity business)
    {
        _context.Set<BusinessEntity>().Add(business);
    }

    public void Update(BusinessEntity business)
    {
        _context.Set<BusinessEntity>().Update(business);
    }
}
