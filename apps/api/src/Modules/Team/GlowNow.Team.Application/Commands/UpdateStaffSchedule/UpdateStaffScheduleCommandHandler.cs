using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.UpdateStaffSchedule;

/// <summary>
/// Handler for the UpdateStaffScheduleCommand.
/// </summary>
internal sealed class UpdateStaffScheduleCommandHandler
    : ICommandHandler<UpdateStaffScheduleCommand>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly ITeamUnitOfWork _unitOfWork;

    public UpdateStaffScheduleCommandHandler(
        IStaffProfileRepository staffProfileRepository,
        ITeamUnitOfWork unitOfWork)
    {
        _staffProfileRepository = staffProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateStaffScheduleCommand command,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(
            command.StaffProfileId, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure(TeamErrors.StaffProfileNotFound);
        }

        // Convert input to WorkDay dictionary
        var schedule = new Dictionary<DayOfWeek, WorkDay?>();
        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            if (!command.Schedule.TryGetValue(day, out var input) || input is null)
            {
                schedule[day] = null;
                continue;
            }

            Result<WorkDay> workDayResult;
            if (input.BreakStart is not null && input.BreakEnd is not null)
            {
                workDayResult = WorkDay.Create(
                    input.StartTime,
                    input.EndTime,
                    input.BreakStart,
                    input.BreakEnd);
            }
            else
            {
                workDayResult = WorkDay.Create(input.StartTime, input.EndTime);
            }

            if (workDayResult.IsFailure)
            {
                return workDayResult;
            }

            schedule[day] = workDayResult.Value;
        }

        var weeklyScheduleResult = WeeklySchedule.Create(schedule);
        if (weeklyScheduleResult.IsFailure)
        {
            return weeklyScheduleResult;
        }

        staffProfile.UpdateSchedule(weeklyScheduleResult.Value);

        _staffProfileRepository.Update(staffProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
