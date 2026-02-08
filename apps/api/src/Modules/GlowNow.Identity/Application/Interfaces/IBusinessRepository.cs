using GlowNow.Business.Domain.Entities;
using GlowNow.Business.Domain.ValueObjects;

namespace GlowNow.Identity.Application.Interfaces;

public interface IBusinessRepository
{
    Task<bool> ExistsByRucAsync(Ruc ruc, CancellationToken cancellationToken = default);
    Task<GlowNow.Business.Domain.Entities.Business?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(GlowNow.Business.Domain.Entities.Business business);
}
