using GlowNow.Identity.Application.Interfaces;
using GlowNow.Identity.Application.Queries.GetCurrentUser;

namespace GlowNow.UnitTests.Identity.Application.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _businessRepository = Substitute.For<IBusinessRepository>();
        _handler = new GetCurrentUserQueryHandler(_userRepository, _businessRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_UserExists()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var user = User.Create(email, "John", "Doe", null, "cognito-id", DateTime.UtcNow);
        var query = new GetCurrentUserQuery(user.Id);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be("test@example.com");
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
        result.Value.Memberships.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetCurrentUserQuery(userId);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.UserNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnMemberships_When_UserHasMemberships()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var user = User.Create(email, "John", "Doe", null, "cognito-id", DateTime.UtcNow);
        var businessId = Guid.NewGuid();
        var membership = BusinessMembership.Create(user.Id, businessId, UserRole.Owner, DateTime.UtcNow);
        user.AddMembership(membership);

        var business = GlowNow.Business.Domain.Entities.Business.Create(
            "Glow Salon",
            Ruc.Create("0102030405001").Value,
            "Cuenca",
            null,
            Email.Create("salon@example.com").Value,
            DateTime.UtcNow);

        var query = new GetCurrentUserQuery(user.Id);
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _businessRepository.GetByIdAsync(businessId, Arg.Any<CancellationToken>()).Returns(business);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Memberships.Should().ContainSingle();
        var membershipResponse = result.Value.Memberships.First();
        membershipResponse.BusinessId.Should().Be(businessId);
        membershipResponse.BusinessName.Should().Be("Glow Salon");
        membershipResponse.Role.Should().Be(UserRole.Owner);
    }

    [Fact]
    public async Task Handle_Should_ReturnUnknownBusinessName_When_BusinessNotFound()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;
        var user = User.Create(email, "John", "Doe", null, "cognito-id", DateTime.UtcNow);
        var businessId = Guid.NewGuid();
        var membership = BusinessMembership.Create(user.Id, businessId, UserRole.Owner, DateTime.UtcNow);
        user.AddMembership(membership);

        var query = new GetCurrentUserQuery(user.Id);
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _businessRepository.GetByIdAsync(businessId, Arg.Any<CancellationToken>())
            .Returns((GlowNow.Business.Domain.Entities.Business)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Memberships.First().BusinessName.Should().Be("Unknown Business");
    }
}
