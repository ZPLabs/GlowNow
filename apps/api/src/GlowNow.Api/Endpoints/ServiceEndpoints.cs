using GlowNow.Api.Infrastructure;
using GlowNow.Catalog.Application.Commands.CreateService;
using GlowNow.Catalog.Application.Commands.DeleteService;
using GlowNow.Catalog.Application.Commands.UpdateService;
using GlowNow.Catalog.Application.Queries.GetAllServices;
using GlowNow.Catalog.Application.Queries.GetService;
using GlowNow.Catalog.Application.Queries.GetServicesByCategory;
using MediatR;

namespace GlowNow.Api.Endpoints;

/// <summary>
/// API endpoints for service management.
/// </summary>
public static class ServiceEndpoints
{
    public static void MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/services")
            .WithTags("Services")
            .RequireAuthorization();

        group.MapPost("/", async (CreateServiceRequest request, ISender sender) =>
        {
            var command = new CreateServiceCommand(
                request.BusinessId,
                request.CategoryId,
                request.Name,
                request.Description,
                request.DurationMinutes,
                request.Price,
                request.BufferTimeMinutes,
                request.DisplayOrder);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("CreateService")
        .WithDescription("Create a new service")
        .Produces<ApiResponse<CreateServiceResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", async (Guid businessId, ISender sender) =>
        {
            var result = await sender.Send(new GetAllServicesQuery(businessId));
            return result.ToApiResponse();
        })
        .WithName("GetAllServices")
        .WithDescription("Get all services for a business")
        .Produces<ApiResponse<IReadOnlyList<ServiceResponse>>>();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetServiceQuery(id));
            return result.ToApiResponse();
        })
        .WithName("GetService")
        .WithDescription("Get a specific service by ID")
        .Produces<ApiResponse<ServiceResponse>>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/by-category/{categoryId:guid}", async (Guid categoryId, ISender sender) =>
        {
            var result = await sender.Send(new GetServicesByCategoryQuery(categoryId));
            return result.ToApiResponse();
        })
        .WithName("GetServicesByCategory")
        .WithDescription("Get all services in a specific category")
        .Produces<ApiResponse<IReadOnlyList<ServiceResponse>>>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", async (Guid id, UpdateServiceRequest request, ISender sender) =>
        {
            var command = new UpdateServiceCommand(
                id,
                request.CategoryId,
                request.Name,
                request.Description,
                request.DurationMinutes,
                request.Price,
                request.BufferTimeMinutes,
                request.DisplayOrder);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("UpdateService")
        .WithDescription("Update an existing service")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteServiceCommand(id));
            return result.ToApiResponse();
        })
        .WithName("DeleteService")
        .WithDescription("Soft-delete a service")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

/// <summary>
/// Request DTO for creating a service.
/// </summary>
public sealed record CreateServiceRequest(
    Guid BusinessId,
    Guid? CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    int BufferTimeMinutes,
    int DisplayOrder);

/// <summary>
/// Request DTO for updating a service.
/// </summary>
public sealed record UpdateServiceRequest(
    Guid? CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    int BufferTimeMinutes,
    int DisplayOrder);
