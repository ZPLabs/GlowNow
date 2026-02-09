namespace GlowNow.UnitTests.Identity.Domain.Entities;

public class BusinessMembershipTests
{
    [Fact]
    public void Create_Should_SetAllPropertiesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var businessId = Guid.NewGuid();
        var role = UserRole.Owner;
        var now = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var membership = BusinessMembership.Create(userId, businessId, role, now);

        // Assert
        membership.Id.Should().NotBe(Guid.Empty);
        membership.UserId.Should().Be(userId);
        membership.BusinessId.Should().Be(businessId);
        membership.Role.Should().Be(UserRole.Owner);
        membership.CreatedAtUtc.Should().Be(now);
    }

    [Fact]
    public void Create_Should_GenerateUniqueIds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var businessId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var membership1 = BusinessMembership.Create(userId, businessId, UserRole.Owner, now);
        var membership2 = BusinessMembership.Create(userId, businessId, UserRole.Staff, now);

        // Assert
        membership1.Id.Should().NotBe(membership2.Id);
    }
}
