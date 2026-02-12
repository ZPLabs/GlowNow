using GlowNow.Identity.Application.Interfaces;
using GlowNow.Identity.Domain.Entities;
using GlowNow.SharedKernel.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Identity.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task<User?> GetByCognitoUserIdAsync(string cognitoUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.CognitoUserId == cognitoUserId, cancellationToken);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }
}
