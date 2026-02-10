using GlowNow.Business.Domain.Errors;
using GlowNow.Business.Domain.Events;
using GlowNow.Business.Domain.ValueObjects;
using GlowNow.Shared.Domain.Errors;
using GlowNow.Shared.Domain.Primitives;
using GlowNow.Shared.Domain.ValueObjects;

namespace GlowNow.Business.Domain.Entities;

/// <summary>
/// Represents a business tenant in the system.
/// </summary>
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
        OperatingHours = OperatingHours.CreateEmpty();
        Description = null;
        LogoUrl = null;
    }

    private Business()
    {
        Name = null!;
        Ruc = null!;
        Address = null!;
        Email = null!;
        OperatingHours = null!;
    } // EF Core

    /// <summary>
    /// Gets the business name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the business RUC (tax identification number).
    /// </summary>
    public Ruc Ruc { get; private set; }

    /// <summary>
    /// Gets the business address.
    /// </summary>
    public string Address { get; private set; }

    /// <summary>
    /// Gets the business phone number.
    /// </summary>
    public PhoneNumber? PhoneNumber { get; private set; }

    /// <summary>
    /// Gets the business email.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the business was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets the business ID for multi-tenancy (same as Id for Business entity).
    /// </summary>
    public Guid BusinessId { get; private set; }

    /// <summary>
    /// Gets the weekly operating hours for the business.
    /// </summary>
    public OperatingHours OperatingHours { get; private set; }

    /// <summary>
    /// Gets the business description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the URL of the business logo.
    /// </summary>
    public string? LogoUrl { get; private set; }

    /// <summary>
    /// Creates a new Business instance.
    /// </summary>
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

    /// <summary>
    /// Sets the operating hours for the business.
    /// </summary>
    /// <param name="operatingHours">The new operating hours.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Result SetOperatingHours(OperatingHours operatingHours)
    {
        if (operatingHours is null)
        {
            return Result.Failure(BusinessErrors.InvalidOperatingHours);
        }

        OperatingHours = operatingHours;
        return Result.Success();
    }

    /// <summary>
    /// Updates the business settings.
    /// </summary>
    /// <param name="name">The new business name.</param>
    /// <param name="description">The new description.</param>
    /// <param name="logoUrl">The new logo URL.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Result UpdateSettings(string name, string? description, string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(BusinessErrors.InvalidBusinessName);
        }

        if (logoUrl is not null && !Uri.TryCreate(logoUrl, UriKind.Absolute, out _))
        {
            return Result.Failure(BusinessErrors.InvalidLogoUrl);
        }

        Name = name.Trim();
        Description = description?.Trim();
        LogoUrl = logoUrl;

        return Result.Success();
    }

    /// <summary>
    /// Updates the business contact information.
    /// </summary>
    /// <param name="address">The new address.</param>
    /// <param name="phoneNumber">The new phone number.</param>
    /// <param name="email">The new email.</param>
    public void UpdateContactInfo(string address, PhoneNumber? phoneNumber, Email email)
    {
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
