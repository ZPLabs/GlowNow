using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.RejectTimeOff;

/// <summary>
/// Handler for the RejectTimeOffCommand.
/// </summary>
internal sealed class RejectTimeOffCommandHandler
    : ICommandHandler<RejectTimeOffCommand>
{
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly ITeamUnitOfWork _unitOfWork;

    public RejectTimeOffCommandHandler(
        ITimeOffRepository timeOffRepository,
        ITeamUnitOfWork unitOfWork)
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
