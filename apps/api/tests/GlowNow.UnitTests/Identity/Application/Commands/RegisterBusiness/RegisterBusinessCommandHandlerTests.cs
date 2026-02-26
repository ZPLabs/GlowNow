using GlowNow.Business.Application.Interfaces;
using GlowNow.Identity.Application.Commands.RegisterBusiness;
using GlowNow.Identity.Application.Interfaces;
using GlowNow.Identity.Domain.Entities;
using GlowNow.Identity.Domain.Enums;
using GlowNow.Identity.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.ValueObjects;
using NSubstitute;
using FluentAssertions;
using Xunit;
using Ruc = GlowNow.Business.Domain.ValueObjects.Ruc;

namespace GlowNow.UnitTests.Identity.Application.Commands.RegisterBusiness;

public class RegisterBusinessCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IBusinessMembershipRepository _membershipRepository;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly IBusinessUnitOfWork _businessUnitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly RegisterBusinessCommandHandler _handler;

    public RegisterBusinessCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _businessRepository = Substitute.For<IBusinessRepository>();
        _membershipRepository = Substitute.For<IBusinessMembershipRepository>();
        _identityUnitOfWork = Substitute.For<IIdentityUnitOfWork>();
        _businessUnitOfWork = Substitute.For<IBusinessUnitOfWork>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _currentUserProvider = Substitute.For<ICurrentUserProvider>();

        _handler = new RegisterBusinessCommandHandler(
            _userRepository,
            _businessRepository,
            _membershipRepository,
            _identityUnitOfWork,
            _businessUnitOfWork,
            _dateTimeProvider,
            _currentUserProvider);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RegistrationIsSuccessful()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        var userId = Guid.NewGuid();
        var user = User.Create(Email.Create("test@example.com").Value, "John", "Doe", null, "cognito-id", DateTime.UtcNow);
        
        _currentUserProvider.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _businessRepository.ExistsByRucAsync(Arg.Any<Ruc>(), Arg.Any<CancellationToken>()).Returns(false);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _identityUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _businessUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RucAlreadyExists()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        _businessRepository.ExistsByRucAsync(Arg.Any<Ruc>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GlowNow.Business.Domain.Errors.BusinessErrors.DuplicateRuc);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        var userId = Guid.NewGuid();
        _currentUserProvider.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User)null!);
        _businessRepository.ExistsByRucAsync(Arg.Any<Ruc>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.UserNotFound);
    }

    [Fact]
    public async Task Handle_Should_CreateMembershipWithOwnerRole()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        var userId = Guid.NewGuid();
        var user = User.Create(Email.Create("test@example.com").Value, "John", "Doe", null, "cognito-id", DateTime.UtcNow);
        
        _currentUserProvider.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _businessRepository.ExistsByRucAsync(Arg.Any<Ruc>(), Arg.Any<CancellationToken>()).Returns(false);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _membershipRepository.Received(1).Add(Arg.Is<BusinessMembership>(m => m.Role == UserRole.Owner));
    }
}
