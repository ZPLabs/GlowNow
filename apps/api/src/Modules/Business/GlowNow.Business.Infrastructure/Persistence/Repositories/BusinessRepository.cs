using GlowNow.Business.Application.Interfaces;
using GlowNow.Business.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using BusinessEntity = GlowNow.Business.Domain.Entities.Business;

namespace GlowNow.Business.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Business aggregate.
/// </summary>
internal sealed class BusinessRepository : IBusinessRepository
{
    private readonly BusinessDbContext _context;

    public BusinessRepository(BusinessDbContext context)
    {
        _context = context;
    }

    public async Task<BusinessEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Businesses
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByRucAsync(Ruc ruc, CancellationToken cancellationToken = default)
    {
        return await _context.Businesses
            .AnyAsync(b => b.Ruc.Value == ruc.Value, cancellationToken);
    }

    public void Add(BusinessEntity business)
    {
        _context.Businesses.Add(business);
    }

    public void Update(BusinessEntity business)
    {
        _context.Businesses.Update(business);
    }
}
