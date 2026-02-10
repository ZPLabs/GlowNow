using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.ApproveTimeOff;

/// <summary>
/// Handler for the ApproveTimeOffCommand.
/// </summary>
internal sealed class ApproveTimeOffCommandHandler
    : ICommandHandler<ApproveTimeOffCommand>
{
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApproveTimeOffCommandHandler(
        ITimeOffRepository timeOffRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _timeOffRepository = timeOffRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(
        ApproveTimeOffCommand command,
        CancellationToken cancellationToken)
    {
        var timeOff = await _timeOffRepository.GetByIdAsync(command.TimeOffId, cancellationToken);

        if (timeOff is null)
        {
            return Result.Failure(TeamErrors.TimeOffNotFound);
        }

        // Check for overlapping approved time off (excluding current request)
        var hasOverlap = await _timeOffRepository.HasOverlappingApprovedTimeOffAsync(
            timeOff.StaffProfileId,
            timeOff.StartDate,
            timeOff.EndDate,
            timeOff.Id,
            cancellationToken);

        if (hasOverlap)
        {
            return Result.Failure(TeamErrors.TimeOffOverlap);
        }

        var result = timeOff.Approve(command.ApprovedByUserId, _dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            return result;
        }

        _timeOffRepository.Update(timeOff);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
