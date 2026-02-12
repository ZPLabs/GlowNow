using GlowNow.Identity.Domain.Enums;
using GlowNow.SharedKernel.Domain.Primitives;

namespace GlowNow.Identity.Domain.Entities;

public sealed class BusinessMembership : Entity<Guid>
{
    private BusinessMembership(
        Guid id,
        Guid userId,
        Guid businessId,
        UserRole role,
        DateTime createdAtUtc)
        : base(id)
    {
        UserId = userId;
        BusinessId = businessId;
        Role = role;
        CreatedAtUtc = createdAtUtc;
    }

    private BusinessMembership() { } // EF Core

    public Guid UserId { get; private set; }
    public Guid BusinessId { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static BusinessMembership Create(
        Guid userId,
        Guid businessId,
        UserRole role,
        DateTime createdAtUtc)
    {
        return new BusinessMembership(
            Guid.NewGuid(),
            userId,
            businessId,
            role,
            createdAtUtc);
    }
}
