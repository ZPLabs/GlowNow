using GlowNow.Business.Application.Interfaces;
using GlowNow.Business.Domain.Errors;
using GlowNow.Identity.Application.Interfaces;
using GlowNow.Identity.Domain.Entities;
using GlowNow.Identity.Domain.Enums;
using GlowNow.Identity.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.ValueObjects;
using BusinessEntity = GlowNow.Business.Domain.Entities.Business;
using RucVO = GlowNow.Business.Domain.ValueObjects.Ruc;

namespace GlowNow.Identity.Application.Commands.RegisterBusiness;

internal sealed class RegisterBusinessCommandHandler : ICommandHandler<RegisterBusinessCommand, RegisterBusinessResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IBusinessMembershipRepository _membershipRepository;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly IBusinessUnitOfWork _businessUnitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserProvider _currentUserProvider;

    public RegisterBusinessCommandHandler(
        IUserRepository userRepository,
        IBusinessRepository businessRepository,
        IBusinessMembershipRepository membershipRepository,
        IIdentityUnitOfWork identityUnitOfWork,
        IBusinessUnitOfWork businessUnitOfWork,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserProvider currentUserProvider)
    {
        _userRepository = userRepository;
        _businessRepository = businessRepository;
        _membershipRepository = membershipRepository;
        _identityUnitOfWork = identityUnitOfWork;
        _businessUnitOfWork = businessUnitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<RegisterBusinessResponse>> Handle(
        RegisterBusinessCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Validate Value Objects
        var rucResult = RucVO.Create(command.BusinessRuc);
        if (rucResult.IsFailure) return Result.Failure<RegisterBusinessResponse>(rucResult.Error);

        PhoneNumber? businessPhoneNumber = null;
        if (command.BusinessPhoneNumber is not null)
        {
            var result = PhoneNumber.Create(command.BusinessPhoneNumber);
            if (result.IsFailure) return Result.Failure<RegisterBusinessResponse>(result.Error);
            businessPhoneNumber = result.Value;
        }

        Email? businessEmail = null;
        if (command.BusinessEmail is not null)
        {
            var result = Email.Create(command.BusinessEmail);
            if (result.IsFailure) return Result.Failure<RegisterBusinessResponse>(result.Error);
            businessEmail = result.Value;
        }

        // 2. Check availability
        if (await _businessRepository.ExistsByRucAsync(rucResult.Value, cancellationToken))
        {
            return Result.Failure<RegisterBusinessResponse>(BusinessErrors.DuplicateRuc);
        }

        // 3. Get existing user (created by middleware)
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<RegisterBusinessResponse>(IdentityErrors.UserNotFound);
        }

        User? user = await _userRepository.GetByIdAsync(_currentUserProvider.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure<RegisterBusinessResponse>(IdentityErrors.UserNotFound);
        }

        DateTime now = _dateTimeProvider.UtcNow;

        // 4. Create business records
        var business = BusinessEntity.Create(
            command.BusinessName,
            rucResult.Value,
            command.BusinessAddress,
            businessPhoneNumber,
            businessEmail ?? user.Email,
            now);

        var membership = BusinessMembership.Create(
            user.Id,
            business.Id,
            UserRole.Owner,
            now);

        user.AddMembership(membership);

        _businessRepository.Add(business);
        _membershipRepository.Add(membership);

        await _identityUnitOfWork.SaveChangesAsync(cancellationToken);
        await _businessUnitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterBusinessResponse(user.Id, business.Id, user.Email);
    }
}
