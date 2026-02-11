using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.CreateBlockedTime;

/// <summary>
/// Handler for the CreateBlockedTimeCommand.
/// </summary>
internal sealed class CreateBlockedTimeCommandHandler
    : ICommandHandler<CreateBlockedTimeCommand, CreateBlockedTimeResponse>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly IBlockedTimeRepository _blockedTimeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateBlockedTimeCommandHandler(
        IStaffProfileRepository staffProfileRepository,
        IBlockedTimeRepository blockedTimeRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _staffProfileRepository = staffProfileRepository;
        _blockedTimeRepository = blockedTimeRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CreateBlockedTimeResponse>> Handle(
        CreateBlockedTimeCommand command,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(
            command.StaffProfileId, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure<CreateBlockedTimeResponse>(TeamErrors.StaffProfileNotFound);
        }

        // Validate required fields based on type
        if (command.IsRecurring && !command.DayOfWeek.HasValue)
        {
            return Result.Failure<CreateBlockedTimeResponse>(TeamErrors.RecurringBlockedTimeMissingDay);
        }

        if (!command.IsRecurring && !command.SpecificDate.HasValue)
        {
            return Result.Failure<CreateBlockedTimeResponse>(TeamErrors.OneTimeBlockedTimeMissingDate);
        }

        // Check for overlapping blocked time
        var hasOverlap = await _blockedTimeRepository.HasOverlappingBlockedTimeAsync(
            command.StaffProfileId,
            command.IsRecurring,
            command.DayOfWeek,
            command.SpecificDate,
            command.StartTime,
            command.EndTime,
            cancellationToken: cancellationToken);

        if (hasOverlap)
        {
            return Result.Failure<CreateBlockedTimeResponse>(TeamErrors.BlockedTimeOverlap);
        }

        Result<BlockedTime> blockedTimeResult;
        if (command.IsRecurring)
        {
            blockedTimeResult = BlockedTime.CreateRecurring(
                command.StaffProfileId,
                staffProfile.BusinessId,
                command.Title,
                command.StartTime,
                command.EndTime,
                command.DayOfWeek!.Value,
                _dateTimeProvider.UtcNow);
        }
        else
        {
            blockedTimeResult = BlockedTime.CreateOneTime(
                command.StaffProfileId,
                staffProfile.BusinessId,
                command.Title,
                command.StartTime,
                command.EndTime,
                command.SpecificDate!.Value,
                _dateTimeProvider.UtcNow);
        }

        if (blockedTimeResult.IsFailure)
        {
            return Result.Failure<CreateBlockedTimeResponse>(blockedTimeResult.Error);
        }

        _blockedTimeRepository.Add(blockedTimeResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBlockedTimeResponse(blockedTimeResult.Value.Id);
    }
}
