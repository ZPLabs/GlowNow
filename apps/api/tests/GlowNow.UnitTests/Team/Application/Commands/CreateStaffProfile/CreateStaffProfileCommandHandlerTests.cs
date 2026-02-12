using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.Team.Application.Commands.CreateStaffProfile;
using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Errors;

namespace GlowNow.UnitTests.Team.Application.Commands.CreateStaffProfile;

public class CreateStaffProfileCommandHandlerTests
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly ITeamUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CreateStaffProfileCommandHandler _handler;

    public CreateStaffProfileCommandHandlerTests()
    {
        _staffProfileRepository = Substitute.For<IStaffProfileRepository>();
        _unitOfWork = Substitute.For<ITeamUnitOfWork>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new CreateStaffProfileCommandHandler(
            _staffProfileRepository,
            _unitOfWork,
            _dateTimeProvider);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCommand()
    {
        // Arrange
        var command = new CreateStaffProfileCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Stylist",
            "Expert",
            "https://image.com",
            1,
            true);

        _staffProfileRepository.ExistsByUserIdAsync(command.BusinessId, command.UserId, Arg.Any<CancellationToken>())
            .Returns(false);
        
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _staffProfileRepository.Received(1).Add(Arg.Is<StaffProfile>(p => 
            p.BusinessId == command.BusinessId && 
            p.UserId == command.UserId &&
            p.Title == command.Title));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_StaffAlreadyExists()
    {
        // Arrange
        var command = new CreateStaffProfileCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Stylist",
            null,
            null,
            0,
            true);

        _staffProfileRepository.ExistsByUserIdAsync(command.BusinessId, command.UserId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.StaffAlreadyExists);
        _staffProfileRepository.DidNotReceive().Add(Arg.Any<StaffProfile>());
    }
}
