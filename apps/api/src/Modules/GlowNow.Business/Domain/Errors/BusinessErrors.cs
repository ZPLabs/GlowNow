using GlowNow.Shared.Domain.Primitives;

namespace GlowNow.Business.Domain.Errors;

/// <summary>
/// Contains all business-related domain errors.
/// </summary>
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

    public static readonly Error InvalidTimeRange = Error.Validation(
        "Business.InvalidTimeRange",
        "The closing time must be after the opening time.");

    public static readonly Error InvalidOperatingHours = Error.Validation(
        "Business.InvalidOperatingHours",
        "The operating hours configuration is invalid.");

    public static readonly Error InvalidBusinessName = Error.Validation(
        "Business.InvalidBusinessName",
        "The business name cannot be empty.");

    public static readonly Error InvalidLogoUrl = Error.Validation(
        "Business.InvalidLogoUrl",
        "The logo URL format is invalid.");
}
