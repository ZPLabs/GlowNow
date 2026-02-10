using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.DeleteBlockedTime;

/// <summary>
/// Handler for the DeleteBlockedTimeCommand.
/// </summary>
internal sealed class DeleteBlockedTimeCommandHandler
    : ICommandHandler<DeleteBlockedTimeCommand>
{
    private readonly IBlockedTimeRepository _blockedTimeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBlockedTimeCommandHandler(
        IBlockedTimeRepository blockedTimeRepository,
        IUnitOfWork unitOfWork)
    {
        _blockedTimeRepository = blockedTimeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteBlockedTimeCommand command,
        CancellationToken cancellationToken)
    {
        var blockedTime = await _blockedTimeRepository.GetByIdAsync(command.Id, cancellationToken);

        if (blockedTime is null)
        {
            return Result.Failure(TeamErrors.BlockedTimeNotFound);
        }

        _blockedTimeRepository.Remove(blockedTime);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
