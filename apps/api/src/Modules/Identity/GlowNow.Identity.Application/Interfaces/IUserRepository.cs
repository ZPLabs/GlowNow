using GlowNow.Identity.Domain.Entities;
using GlowNow.SharedKernel.Domain.ValueObjects;

namespace GlowNow.Identity.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetByCognitoUserIdAsync(string cognitoUserId, CancellationToken cancellationToken = default);
    void Add(User user);
}
