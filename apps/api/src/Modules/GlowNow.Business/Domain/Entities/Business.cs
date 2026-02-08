using GlowNow.Business.Domain.Events;
using GlowNow.Business.Domain.ValueObjects;
using GlowNow.Shared.Domain.Primitives;
using GlowNow.Shared.Domain.ValueObjects;

namespace GlowNow.Business.Domain.Entities;

public sealed class Business : AggregateRoot<Guid>, ITenantScoped
{
    private Business(
        Guid id,
        string name,
        Ruc ruc,
        string address,
        PhoneNumber? phoneNumber,
        Email email,
        DateTime createdAtUtc)
        : base(id)
    {
        Name = name;
        Ruc = ruc;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        CreatedAtUtc = createdAtUtc;
        BusinessId = id; // TenantId is the BusinessId itself
    }

    private Business() 
    {
        Name = null!;
        Ruc = null!;
        Address = null!;
        Email = null!;
    } // EF Core

    public string Name { get; private set; }
    public Ruc Ruc { get; private set; }
    public string Address { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Email Email { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid BusinessId { get; private set; }

    public static Business Create(
        string name,
        Ruc ruc,
        string address,
        PhoneNumber? phoneNumber,
        Email email,
        DateTime createdAtUtc)
    {
        var business = new Business(
            Guid.NewGuid(),
            name,
            ruc,
            address,
            phoneNumber,
            email,
            createdAtUtc);

        business.RaiseDomainEvent(new BusinessRegisteredEvent(business.Id, business.Name, business.Ruc.Value));

        return business;
    }
}
