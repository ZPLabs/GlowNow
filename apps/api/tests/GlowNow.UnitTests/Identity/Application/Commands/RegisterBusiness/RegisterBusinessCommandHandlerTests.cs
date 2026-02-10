using GlowNow.Business.Application.Interfaces;
using GlowNow.Identity.Application.Commands.RegisterBusiness;
using GlowNow.Identity.Application.Interfaces;
using GlowNow.Shared.Application.Interfaces;

namespace GlowNow.UnitTests.Identity.Application.Commands.RegisterBusiness;

public class RegisterBusinessCommandHandlerTests
{
    private readonly ICognitoIdentityProvider _cognitoService;
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IBusinessMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly RegisterBusinessCommandHandler _handler;

    public RegisterBusinessCommandHandlerTests()
    {
        _cognitoService = Substitute.For<ICognitoIdentityProvider>();
        _userRepository = Substitute.For<IUserRepository>();
        _businessRepository = Substitute.For<IBusinessRepository>();
        _membershipRepository = Substitute.For<IBusinessMembershipRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        
        _handler = new RegisterBusinessCommandHandler(
            _cognitoService,
            _userRepository,
            _businessRepository,
            _membershipRepository,
            _unitOfWork,
            _dateTimeProvider);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RegistrationIsSuccessful()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "Password123!", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        _userRepository.GetByEmailAsync(Arg.Any<Email>()).Returns((User)null!);
        _businessRepository.ExistsByRucAsync(Arg.Any<Ruc>()).Returns(false);
        _cognitoService.RegisterUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDictionary<string, string>>())
            .Returns(Result.Success("cognito-id"));
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EmailAlreadyInUse()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "Password123!", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        var existingUser = User.Create(Email.Create(command.Email).Value, "Ex", "isting", null, "id", DateTime.UtcNow);
        _userRepository.GetByEmailAsync(Arg.Any<Email>()).Returns(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.EmailAlreadyInUse);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RucAlreadyExists()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "Password123!", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        _userRepository.GetByEmailAsync(Arg.Any<Email>()).Returns((User)null!);
        _businessRepository.ExistsByRucAsync(Arg.Any<Ruc>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GlowNow.Business.Domain.Errors.BusinessErrors.DuplicateRuc);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CognitoRegistrationFails()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "Password123!", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        _userRepository.GetByEmailAsync(Arg.Any<Email>()).Returns((User)null!);
        _businessRepository.ExistsByRucAsync(Arg.Any<Ruc>()).Returns(false);
        _cognitoService.RegisterUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDictionary<string, string>>())
            .Returns(Result.Failure<string>(IdentityErrors.CognitoError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.CognitoError);
    }

    [Fact]
    public async Task Handle_Should_DeleteCognitoUser_When_DbSaveFails()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "Password123!", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        _userRepository.GetByEmailAsync(Arg.Any<Email>()).Returns((User)null!);
        _businessRepository.ExistsByRucAsync(Arg.Any<Ruc>()).Returns(false);
        _cognitoService.RegisterUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDictionary<string, string>>())
            .Returns(Result.Success("cognito-id"));
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns<int>(_ => throw new Exception("DB failure"));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("DB failure");
        await _cognitoService.Received(1).DeleteUserAsync("cognito-id");
    }

    [Fact]
    public async Task Handle_Should_CreateMembershipWithOwnerRole()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "Password123!", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        _userRepository.GetByEmailAsync(Arg.Any<Email>()).Returns((User)null!);
        _businessRepository.ExistsByRucAsync(Arg.Any<Ruc>()).Returns(false);
        _cognitoService.RegisterUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDictionary<string, string>>())
            .Returns(Result.Success("cognito-id"));
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _membershipRepository.Received(1).Add(Arg.Is<BusinessMembership>(m => m.Role == UserRole.Owner));
    }
}
