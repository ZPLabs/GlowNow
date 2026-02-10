using GlowNow.Api.Infrastructure;
using GlowNow.Catalog.Application.Commands.CreateServiceCategory;
using GlowNow.Catalog.Application.Commands.DeleteServiceCategory;
using GlowNow.Catalog.Application.Commands.UpdateServiceCategory;
using GlowNow.Catalog.Application.Queries.GetAllCategories;
using MediatR;

namespace GlowNow.Api.Endpoints;

/// <summary>
/// API endpoints for service category management.
/// </summary>
public static class ServiceCategoryEndpoints
{
    public static void MapServiceCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/services/categories")
            .WithTags("Service Categories")
            .RequireAuthorization();

        group.MapPost("/", async (CreateServiceCategoryRequest request, ISender sender) =>
        {
            var command = new CreateServiceCategoryCommand(
                request.BusinessId,
                request.Name,
                request.Description,
                request.DisplayOrder);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("CreateServiceCategory")
        .WithDescription("Create a new service category")
        .Produces<ApiResponse<CreateServiceCategoryResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", async (Guid businessId, ISender sender) =>
        {
            var result = await sender.Send(new GetAllCategoriesQuery(businessId));
            return result.ToApiResponse();
        })
        .WithName("GetAllCategories")
        .WithDescription("Get all service categories for a business")
        .Produces<ApiResponse<IReadOnlyList<ServiceCategoryResponse>>>();

        group.MapPut("/{id:guid}", async (Guid id, UpdateServiceCategoryRequest request, ISender sender) =>
        {
            var command = new UpdateServiceCategoryCommand(
                id,
                request.Name,
                request.Description,
                request.DisplayOrder);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("UpdateServiceCategory")
        .WithDescription("Update an existing service category")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteServiceCategoryCommand(id));
            return result.ToApiResponse();
        })
        .WithName("DeleteServiceCategory")
        .WithDescription("Soft-delete a service category")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

/// <summary>
/// Request DTO for creating a service category.
/// </summary>
public sealed record CreateServiceCategoryRequest(
    Guid BusinessId,
    string Name,
    string? Description,
    int DisplayOrder);

/// <summary>
/// Request DTO for updating a service category.
/// </summary>
public sealed record UpdateServiceCategoryRequest(
    string Name,
    string? Description,
    int DisplayOrder);
