namespace GlowNow.Shared.Domain.Errors;

/// <summary>
/// Represents a validation error containing multiple sub-errors.
/// </summary>
public sealed record ValidationError(Error[] Errors) 
    : Error("Validation.Error", "One or more validation errors occurred.")
{
    public static ValidationError FromErrors(IEnumerable<Error> errors) => new(errors.ToArray());
}
