using GlowNow.Business.Application.Interfaces;
using GlowNow.Identity.Application.Interfaces;
using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using BusinessEntity = GlowNow.Business.Domain.Entities.Business;
using RucVO = GlowNow.Business.Domain.ValueObjects.Ruc;

namespace GlowNow.Identity.Application.Commands.RegisterBusiness;

internal sealed class RegisterBusinessCommandHandler : ICommandHandler<RegisterBusinessCommand, RegisterBusinessResponse>
{
    private readonly ICognitoIdentityProvider _cognitoService;
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IBusinessMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterBusinessCommandHandler(
        ICognitoIdentityProvider cognitoService,
        IUserRepository userRepository,
        IBusinessRepository businessRepository,
        IBusinessMembershipRepository membershipRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _cognitoService = cognitoService;
        _userRepository = userRepository;
        _businessRepository = businessRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<RegisterBusinessResponse>> Handle(
        RegisterBusinessCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Validate Value Objects
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure) return Result.Failure<RegisterBusinessResponse>(emailResult.Error);

        var rucResult = RucVO.Create(command.BusinessRuc);
        if (rucResult.IsFailure) return Result.Failure<RegisterBusinessResponse>(rucResult.Error);

        PhoneNumber? userPhoneNumber = null;
        if (command.PhoneNumber is not null)
        {
            var result = PhoneNumber.Create(command.PhoneNumber);
            if (result.IsFailure) return Result.Failure<RegisterBusinessResponse>(result.Error);
            userPhoneNumber = result.Value;
        }

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
        if (await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken) is not null)
        {
            return Result.Failure<RegisterBusinessResponse>(IdentityErrors.EmailAlreadyInUse);
        }

        if (await _businessRepository.ExistsByRucAsync(rucResult.Value, cancellationToken))
        {
            // Assuming BusinessErrors is available in Identity via global usings or direct access
            // Since we didn't add BusinessErrors to Identity GlobalUsings, let's use it directly
            return Result.Failure<RegisterBusinessResponse>(GlowNow.Business.Domain.Errors.BusinessErrors.DuplicateRuc);
        }

        // 3. Create Cognito User
        var attributes = new Dictionary<string, string>
        {
            { "email", emailResult.Value },
            { "given_name", command.FirstName },
            { "family_name", command.LastName }
        };

        var cognitoResult = await _cognitoService.RegisterUserAsync(command.Email, command.Password, attributes);
        if (cognitoResult.IsFailure)
        {
            return Result.Failure<RegisterBusinessResponse>(cognitoResult.Error);
        }

        string cognitoUserId = cognitoResult.Value;

        try
        {
            DateTime now = _dateTimeProvider.UtcNow;

            // 4. Create local records
            var user = User.Create(
                emailResult.Value,
                command.FirstName,
                command.LastName,
                userPhoneNumber,
                cognitoUserId,
                now);

            var business = BusinessEntity.Create(
                command.BusinessName,
                rucResult.Value,
                command.BusinessAddress,
                businessPhoneNumber,
                businessEmail ?? emailResult.Value,
                now);

            var membership = BusinessMembership.Create(
                user.Id,
                business.Id,
                UserRole.Owner,
                now);

            user.AddMembership(membership);

            _userRepository.Add(user);
            _businessRepository.Add(business);
            _membershipRepository.Add(membership);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterBusinessResponse(user.Id, business.Id, user.Email);
        }
        catch
        {
            // 5. Compensating action: Delete Cognito user if local DB fails
            await _cognitoService.DeleteUserAsync(cognitoUserId);
            throw;
        }
    }
}
