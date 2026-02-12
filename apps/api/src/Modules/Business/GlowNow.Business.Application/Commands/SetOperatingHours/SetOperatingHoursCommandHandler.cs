using GlowNow.Business.Application.Interfaces;
using GlowNow.Business.Domain.Errors;
using GlowNow.Business.Domain.ValueObjects;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Business.Application.Commands.SetOperatingHours;

/// <summary>
/// Handler for the SetOperatingHoursCommand.
/// </summary>
internal sealed class SetOperatingHoursCommandHandler : ICommandHandler<SetOperatingHoursCommand>
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IBusinessUnitOfWork _unitOfWork;

    public SetOperatingHoursCommandHandler(
        IBusinessRepository businessRepository,
        IBusinessUnitOfWork unitOfWork)
    {
        _businessRepository = businessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SetOperatingHoursCommand command, CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetByIdAsync(command.BusinessId, cancellationToken);

        if (business is null)
        {
            return Result.Failure(BusinessErrors.BusinessNotFound);
        }

        // Convert DTOs to TimeRange value objects
        var schedule = new Dictionary<DayOfWeek, TimeRange?>();

        foreach (var kvp in command.Schedule)
        {
            if (kvp.Value is null)
            {
                schedule[kvp.Key] = null;
                continue;
            }

            var timeRangeResult = TimeRange.Create(kvp.Value.OpenTime, kvp.Value.CloseTime);
            if (timeRangeResult.IsFailure)
            {
                return Result.Failure(timeRangeResult.Error);
            }

            schedule[kvp.Key] = timeRangeResult.Value;
        }

        var operatingHoursResult = OperatingHours.Create(schedule);
        if (operatingHoursResult.IsFailure)
        {
            return Result.Failure(operatingHoursResult.Error);
        }

        var setResult = business.SetOperatingHours(operatingHoursResult.Value);
        if (setResult.IsFailure)
        {
            return setResult;
        }

        _businessRepository.Update(business);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
