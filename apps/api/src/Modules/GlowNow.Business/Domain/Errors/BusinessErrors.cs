using GlowNow.Shared.Domain.Primitives;

namespace GlowNow.Business.Domain.Errors;

public static class BusinessErrors
{
    public static readonly Error DuplicateRuc = Error.Conflict(
        "Business.DuplicateRuc",
        "A business with the same RUC already exists.");

    public static readonly Error BusinessNotFound = Error.NotFound(
        "Business.NotFound",
        "The business with the specified identifier was not found.");

    public static readonly Error InvalidRuc = Error.Validation(
        "Business.InvalidRuc",
        "The provided RUC is invalid.");
}
