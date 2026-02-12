using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.CreateStaffProfile;

/// <summary>
/// Handler for the CreateStaffProfileCommand.
/// </summary>
internal sealed class CreateStaffProfileCommandHandler
    : ICommandHandler<CreateStaffProfileCommand, CreateStaffProfileResponse>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly ITeamUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateStaffProfileCommandHandler(
        IStaffProfileRepository staffProfileRepository,
        ITeamUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _staffProfileRepository = staffProfileRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CreateStaffProfileResponse>> Handle(
        CreateStaffProfileCommand command,
        CancellationToken cancellationToken)
    {
        // Check if staff profile already exists for this user in this business
        if (await _staffProfileRepository.ExistsByUserIdAsync(
            command.BusinessId, command.UserId, cancellationToken))
        {
            return Result.Failure<CreateStaffProfileResponse>(TeamErrors.StaffAlreadyExists);
        }

        // Create the staff profile
        var profileResult = StaffProfile.Create(
            command.BusinessId,
            command.UserId,
            command.Title,
            command.Bio,
            command.ProfileImageUrl,
            command.DisplayOrder,
            command.AcceptsOnlineBookings,
            _dateTimeProvider.UtcNow);

        if (profileResult.IsFailure)
        {
            return Result.Failure<CreateStaffProfileResponse>(profileResult.Error);
        }

        _staffProfileRepository.Add(profileResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateStaffProfileResponse(profileResult.Value.Id);
    }
}
