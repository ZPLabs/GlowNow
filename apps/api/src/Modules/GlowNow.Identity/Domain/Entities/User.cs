using GlowNow.Identity.Domain.Events;
using GlowNow.Shared.Domain.Primitives;
using GlowNow.Shared.Domain.ValueObjects;

namespace GlowNow.Identity.Domain.Entities;

public sealed class User : AggregateRoot<Guid>
{
    private readonly List<BusinessMembership> _memberships = new();

    private User(
        Guid id,
        Email email,
        string firstName,
        string lastName,
        PhoneNumber? phoneNumber,
        string cognitoUserId,
        DateTime createdAtUtc)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CognitoUserId = cognitoUserId;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    private User() 
    {
        Email = null!;
        FirstName = null!;
        LastName = null!;
        CognitoUserId = null!;
    } // EF Core

    public Email Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public string CognitoUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<BusinessMembership> Memberships => _memberships.AsReadOnly();

    public static User Create(
        Email email,
        string firstName,
        string lastName,
        PhoneNumber? phoneNumber,
        string cognitoUserId,
        DateTime createdAtUtc)
    {
        var user = new User(
            Guid.NewGuid(),
            email,
            firstName,
            lastName,
            phoneNumber,
            cognitoUserId,
            createdAtUtc);

        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.FirstName));

        return user;
    }

    public void AddMembership(BusinessMembership membership)
    {
        _memberships.Add(membership);
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
