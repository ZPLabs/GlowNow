using GlowNow.Business.Application.Interfaces;
using GlowNow.Business.Domain.Errors;
using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Shared.Domain.Errors;

namespace GlowNow.Business.Application.Commands.UpdateBusinessSettings;

/// <summary>
/// Handler for the UpdateBusinessSettingsCommand.
/// </summary>
internal sealed class UpdateBusinessSettingsCommandHandler : ICommandHandler<UpdateBusinessSettingsCommand>
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusinessSettingsCommandHandler(
        IBusinessRepository businessRepository,
        IUnitOfWork unitOfWork)
    {
        _businessRepository = businessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateBusinessSettingsCommand command, CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetByIdAsync(command.BusinessId, cancellationToken);

        if (business is null)
        {
            return Result.Failure(BusinessErrors.BusinessNotFound);
        }

        var updateResult = business.UpdateSettings(command.Name, command.Description, command.LogoUrl);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        _businessRepository.Update(business);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
