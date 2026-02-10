using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.CancelTimeOff;

/// <summary>
/// Handler for the CancelTimeOffCommand.
/// </summary>
internal sealed class CancelTimeOffCommandHandler
    : ICommandHandler<CancelTimeOffCommand>
{
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CancelTimeOffCommandHandler(
        ITimeOffRepository timeOffRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _timeOffRepository = timeOffRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(
        CancelTimeOffCommand command,
        CancellationToken cancellationToken)
    {
        var timeOff = await _timeOffRepository.GetByIdAsync(command.TimeOffId, cancellationToken);

        if (timeOff is null)
        {
            return Result.Failure(TeamErrors.TimeOffNotFound);
        }

        var currentDate = DateOnly.FromDateTime(_dateTimeProvider.UtcNow);
        var result = timeOff.Cancel(currentDate);
        if (result.IsFailure)
        {
            return result;
        }

        _timeOffRepository.Update(timeOff);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
