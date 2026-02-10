using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.UnassignServiceFromStaff;

/// <summary>
/// Handler for the UnassignServiceFromStaffCommand.
/// </summary>
internal sealed class UnassignServiceFromStaffCommandHandler
    : ICommandHandler<UnassignServiceFromStaffCommand>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnassignServiceFromStaffCommandHandler(
        IStaffProfileRepository staffProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _staffProfileRepository = staffProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UnassignServiceFromStaffCommand command,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdWithServicesAsync(
            command.StaffProfileId, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure(TeamErrors.StaffProfileNotFound);
        }

        var result = staffProfile.UnassignService(command.ServiceId);
        if (result.IsFailure)
        {
            return result;
        }

        _staffProfileRepository.Update(staffProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
