namespace GlowNow.UnitTests.Identity.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Create_Should_RaiseUserRegisteredEvent()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var now = DateTime.UtcNow;

        // Act
        var user = User.Create(email, "John", "Doe", null, "cognito-id", now);

        // Assert
        user.GetDomainEvents().Should().ContainSingle(e => e is UserRegisteredEvent);
    }

    [Fact]
    public void Create_Should_SetAllPropertiesCorrectly()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var phone = PhoneNumber.Create("+593987654321").Value;
        var now = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var user = User.Create(email, "John", "Doe", phone, "cognito-sub-123", now);

        // Assert
        user.Id.Should().NotBe(Guid.Empty);
        user.Email.Value.Should().Be("test@example.com");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.PhoneNumber!.Value.Should().Be("+593987654321");
        user.CognitoUserId.Should().Be("cognito-sub-123");
        user.CreatedAtUtc.Should().Be(now);
        user.UpdatedAtUtc.Should().Be(now);
    }

    [Fact]
    public void Create_Should_SetPhoneNumberToNull_When_NotProvided()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var now = DateTime.UtcNow;

        // Act
        var user = User.Create(email, "John", "Doe", null, "cognito-id", now);

        // Assert
        user.PhoneNumber.Should().BeNull();
    }

    [Fact]
    public void Create_Should_InitializeEmptyMemberships()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var now = DateTime.UtcNow;

        // Act
        var user = User.Create(email, "John", "Doe", null, "cognito-id", now);

        // Assert
        user.Memberships.Should().BeEmpty();
    }

    [Fact]
    public void AddMembership_Should_AddToCollection()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var now = DateTime.UtcNow;
        var user = User.Create(email, "John", "Doe", null, "cognito-id", now);
        var membership = BusinessMembership.Create(user.Id, Guid.NewGuid(), UserRole.Owner, now);

        // Act
        user.AddMembership(membership);

        // Assert
        user.Memberships.Should().ContainSingle();
        user.Memberships.First().Should().Be(membership);
    }

    [Fact]
    public void AddMembership_Should_UpdateUpdatedAtUtc()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var createdAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var user = User.Create(email, "John", "Doe", null, "cognito-id", createdAt);
        var membership = BusinessMembership.Create(user.Id, Guid.NewGuid(), UserRole.Owner, DateTime.UtcNow);

        // Act
        user.AddMembership(membership);

        // Assert
        user.UpdatedAtUtc.Should().BeAfter(createdAt);
    }
}
