using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.RequestTimeOff;

/// <summary>
/// Handler for the RequestTimeOffCommand.
/// </summary>
internal sealed class RequestTimeOffCommandHandler
    : ICommandHandler<RequestTimeOffCommand, RequestTimeOffResponse>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RequestTimeOffCommandHandler(
        IStaffProfileRepository staffProfileRepository,
        ITimeOffRepository timeOffRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _staffProfileRepository = staffProfileRepository;
        _timeOffRepository = timeOffRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<RequestTimeOffResponse>> Handle(
        RequestTimeOffCommand command,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(
            command.StaffProfileId, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure<RequestTimeOffResponse>(TeamErrors.StaffProfileNotFound);
        }

        // Check for overlapping approved time off
        var hasOverlap = await _timeOffRepository.HasOverlappingApprovedTimeOffAsync(
            command.StaffProfileId,
            command.StartDate,
            command.EndDate,
            cancellationToken: cancellationToken);

        if (hasOverlap)
        {
            return Result.Failure<RequestTimeOffResponse>(TeamErrors.TimeOffOverlap);
        }

        var timeOffResult = TimeOff.Create(
            command.StaffProfileId,
            staffProfile.BusinessId,
            command.StartDate,
            command.EndDate,
            command.Type,
            command.Notes,
            _dateTimeProvider.UtcNow);

        if (timeOffResult.IsFailure)
        {
            return Result.Failure<RequestTimeOffResponse>(timeOffResult.Error);
        }

        _timeOffRepository.Add(timeOffResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RequestTimeOffResponse(timeOffResult.Value.Id);
    }
}
