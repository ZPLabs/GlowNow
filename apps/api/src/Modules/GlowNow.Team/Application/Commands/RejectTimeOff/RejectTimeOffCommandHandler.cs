using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.RejectTimeOff;

/// <summary>
/// Handler for the RejectTimeOffCommand.
/// </summary>
internal sealed class RejectTimeOffCommandHandler
    : ICommandHandler<RejectTimeOffCommand>
{
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectTimeOffCommandHandler(
        ITimeOffRepository timeOffRepository,
        IUnitOfWork unitOfWork)
    {
        _timeOffRepository = timeOffRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RejectTimeOffCommand command,
        CancellationToken cancellationToken)
    {
        var timeOff = await _timeOffRepository.GetByIdAsync(command.TimeOffId, cancellationToken);

        if (timeOff is null)
        {
            return Result.Failure(TeamErrors.TimeOffNotFound);
        }

        var result = timeOff.Reject(command.Reason);
        if (result.IsFailure)
        {
            return result;
        }

        _timeOffRepository.Update(timeOff);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
