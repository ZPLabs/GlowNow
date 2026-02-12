using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.UpdateStaffProfile;

/// <summary>
/// Handler for the UpdateStaffProfileCommand.
/// </summary>
internal sealed class UpdateStaffProfileCommandHandler
    : ICommandHandler<UpdateStaffProfileCommand>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly ITeamUnitOfWork _unitOfWork;

    public UpdateStaffProfileCommandHandler(
        IStaffProfileRepository staffProfileRepository,
        ITeamUnitOfWork unitOfWork)
    {
        _staffProfileRepository = staffProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateStaffProfileCommand command,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(command.Id, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure(TeamErrors.StaffProfileNotFound);
        }

        var result = staffProfile.Update(
            command.Title,
            command.Bio,
            command.ProfileImageUrl,
            command.DisplayOrder,
            command.AcceptsOnlineBookings);

        if (result.IsFailure)
        {
            return result;
        }

        _staffProfileRepository.Update(staffProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
