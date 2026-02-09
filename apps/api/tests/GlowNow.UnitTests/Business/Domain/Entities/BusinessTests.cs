using GlowNow.Business.Domain.Events;

namespace GlowNow.UnitTests.Business.Domain.Entities;

public class BusinessTests
{
    [Fact]
    public void Create_Should_SetAllPropertiesCorrectly()
    {
        // Arrange
        var ruc = Ruc.Create("0102030405001").Value;
        var email = Email.Create("salon@example.com").Value;
        var phone = PhoneNumber.Create("+593987654321").Value;
        var now = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var business = GlowNow.Business.Domain.Entities.Business.Create(
            "Glow Salon", ruc, "Cuenca", phone, email, now);

        // Assert
        business.Id.Should().NotBe(Guid.Empty);
        business.Name.Should().Be("Glow Salon");
        business.Ruc.Value.Should().Be("0102030405001");
        business.Address.Should().Be("Cuenca");
        business.PhoneNumber!.Value.Should().Be("+593987654321");
        business.Email.Value.Should().Be("salon@example.com");
        business.CreatedAtUtc.Should().Be(now);
    }

    [Fact]
    public void Create_Should_SetBusinessIdToOwnId()
    {
        // Arrange
        var ruc = Ruc.Create("0102030405001").Value;
        var email = Email.Create("salon@example.com").Value;
        var now = DateTime.UtcNow;

        // Act
        var business = GlowNow.Business.Domain.Entities.Business.Create(
            "Glow Salon", ruc, "Cuenca", null, email, now);

        // Assert
        business.BusinessId.Should().Be(business.Id);
    }

    [Fact]
    public void Create_Should_RaiseBusinessRegisteredEvent()
    {
        // Arrange
        var ruc = Ruc.Create("0102030405001").Value;
        var email = Email.Create("salon@example.com").Value;
        var now = DateTime.UtcNow;

        // Act
        var business = GlowNow.Business.Domain.Entities.Business.Create(
            "Glow Salon", ruc, "Cuenca", null, email, now);

        // Assert
        business.GetDomainEvents().Should().ContainSingle(e => e is BusinessRegisteredEvent);
    }

    [Fact]
    public void Create_Should_AllowNullPhoneNumber()
    {
        // Arrange
        var ruc = Ruc.Create("0102030405001").Value;
        var email = Email.Create("salon@example.com").Value;
        var now = DateTime.UtcNow;

        // Act
        var business = GlowNow.Business.Domain.Entities.Business.Create(
            "Glow Salon", ruc, "Cuenca", null, email, now);

        // Assert
        business.PhoneNumber.Should().BeNull();
    }
}
