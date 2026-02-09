using GlowNow.Identity.Application.Interfaces;
using GlowNow.Shared.Infrastructure.Persistence;

namespace GlowNow.Identity.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>().FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task<User?> GetByCognitoUserIdAsync(string cognitoUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>().FirstOrDefaultAsync(u => u.CognitoUserId == cognitoUserId, cancellationToken);
    }

    public void Add(User user)
    {
        _context.Set<User>().Add(user);
    }
}
