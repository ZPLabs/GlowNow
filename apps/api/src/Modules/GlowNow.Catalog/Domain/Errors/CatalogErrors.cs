using GlowNow.Shared.Domain.Primitives;

namespace GlowNow.Catalog.Domain.Errors;

/// <summary>
/// Contains all catalog-related domain errors.
/// </summary>
public static class CatalogErrors
{
    public static readonly Error ServiceNotFound = Error.NotFound(
        "Catalog.ServiceNotFound",
        "The service with the specified identifier was not found.");

    public static readonly Error CategoryNotFound = Error.NotFound(
        "Catalog.CategoryNotFound",
        "The service category with the specified identifier was not found.");

    public static readonly Error DuplicateServiceName = Error.Conflict(
        "Catalog.DuplicateServiceName",
        "A service with the same name already exists in this business.");

    public static readonly Error DuplicateCategoryName = Error.Conflict(
        "Catalog.DuplicateCategoryName",
        "A category with the same name already exists in this business.");

    public static readonly Error InvalidPrice = Error.Validation(
        "Catalog.InvalidPrice",
        "The price must be a non-negative value up to $999,999.99.");

    public static Error InvalidDuration(int min, int max) => Error.Validation(
        "Catalog.InvalidDuration",
        $"The duration must be between {min} and {max} minutes.");

    public static readonly Error InvalidServiceName = Error.Validation(
        "Catalog.InvalidServiceName",
        "The service name cannot be empty.");

    public static readonly Error InvalidCategoryName = Error.Validation(
        "Catalog.InvalidCategoryName",
        "The category name cannot be empty.");

    public static readonly Error CategoryHasServices = Error.Conflict(
        "Catalog.CategoryHasServices",
        "Cannot delete a category that contains services. Move or delete the services first.");

    public static readonly Error ServiceIsActive = Error.Conflict(
        "Catalog.ServiceIsActive",
        "Cannot delete an active service. Deactivate it first.");
}
