using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.DeactivateStaff;

/// <summary>
/// Handler for the DeactivateStaffCommand.
/// </summary>
internal sealed class DeactivateStaffCommandHandler
    : ICommandHandler<DeactivateStaffCommand>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateStaffCommandHandler(
        IStaffProfileRepository staffProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _staffProfileRepository = staffProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeactivateStaffCommand command,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(command.Id, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure(TeamErrors.StaffProfileNotFound);
        }

        var result = staffProfile.Deactivate();
        if (result.IsFailure)
        {
            return result;
        }

        _staffProfileRepository.Update(staffProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
