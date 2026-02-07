using GlowNow.Identity;
using GlowNow.Business;
using GlowNow.Catalog;
using GlowNow.Team;
using GlowNow.Clients;
using GlowNow.Booking;
using GlowNow.Notifications;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddIdentityModule()
    .AddBusinessModule()
    .AddCatalogModule()
    .AddTeamModule()
    .AddClientsModule()
    .AddBookingModule()
    .AddNotificationsModule();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
