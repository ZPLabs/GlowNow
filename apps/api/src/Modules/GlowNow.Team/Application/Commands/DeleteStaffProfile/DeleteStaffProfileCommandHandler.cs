using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.DeleteStaffProfile;

/// <summary>
/// Handler for the DeleteStaffProfileCommand.
/// </summary>
internal sealed class DeleteStaffProfileCommandHandler
    : ICommandHandler<DeleteStaffProfileCommand>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStaffProfileCommandHandler(
        IStaffProfileRepository staffProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _staffProfileRepository = staffProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteStaffProfileCommand command,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(command.Id, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure(TeamErrors.StaffProfileNotFound);
        }

        var result = staffProfile.Delete();
        if (result.IsFailure)
        {
            return result;
        }

        _staffProfileRepository.Update(staffProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
