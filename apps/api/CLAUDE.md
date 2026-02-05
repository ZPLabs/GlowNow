# CLAUDE.md - API Context

## Tech Stack
- **Framework:** .NET 10 (ASP.NET Core Web API)
- **Language:** C# 13+
- **Architecture:** Clean Architecture (Monolithic)
  - `GlowNow.Api`: Presentation layer (Controllers, Endpoints).
  - `GlowNow.Application`: Business logic, CQRS (MediatR pattern likely), DTOs, Interfaces.
  - `GlowNow.Domain`: Core entities, Value objects, Domain events, Repository interfaces.
  - `GlowNow.Infrastructure`: Data access (EF Core), External services implementation.

## Development Standards
- **Naming:** PascalCase for public members/types. camelCase for local variables/parameters.
- **Dependency Injection:** Use constructor injection. Register services in `DependencyInjection.cs` in each layer.
- **Controllers:** Keep controllers thin; delegate logic to Application layer (MediatR Handlers).
- **Async:** Always use `async/await` all the way down.
- **Nullability:** Nullable Reference Types are enabled. Handle potential nulls explicitly.

## Commands
```bash
# Run API (from apps/api)
dotnet run --project src/GlowNow.Api

# Build Solution
dotnet build

# Add Migration (if EF Core is used)
dotnet ef migrations add <MigrationName> --project src/GlowNow.Infrastructure --startup-project src/GlowNow.Api
```
