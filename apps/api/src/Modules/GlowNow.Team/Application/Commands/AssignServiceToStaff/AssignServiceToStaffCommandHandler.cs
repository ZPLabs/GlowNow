using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Commands.AssignServiceToStaff;

/// <summary>
/// Handler for the AssignServiceToStaffCommand.
/// </summary>
internal sealed class AssignServiceToStaffCommandHandler
    : ICommandHandler<AssignServiceToStaffCommand>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly IServiceValidator _serviceValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AssignServiceToStaffCommandHandler(
        IStaffProfileRepository staffProfileRepository,
        IServiceValidator serviceValidator,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _staffProfileRepository = staffProfileRepository;
        _serviceValidator = serviceValidator;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(
        AssignServiceToStaffCommand command,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdWithServicesAsync(
            command.StaffProfileId, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure(TeamErrors.StaffProfileNotFound);
        }

        // Validate that the service exists and belongs to the same business
        var serviceExists = await _serviceValidator.ValidateServiceExistsAsync(
            command.ServiceId, staffProfile.BusinessId, cancellationToken);

        if (!serviceExists)
        {
            return Result.Failure(TeamErrors.ServiceNotFound);
        }

        var result = staffProfile.AssignService(command.ServiceId, _dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            return result;
        }

        _staffProfileRepository.Update(staffProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
