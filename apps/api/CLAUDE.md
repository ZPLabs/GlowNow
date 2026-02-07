# CLAUDE.md - API Context

## Tech Stack
- **Framework:** .NET 10 (Minimal APIs)
- **Language:** C# 14
- **Architecture:** Clean Architecture (Monolithic) within a Turborepo monorepo.
  - `GlowNow.Api`: Presentation layer (Minimal API Endpoints, DI).
  - `GlowNow.Application`: Business logic, CQRS (MediatR pattern), Use cases, Interfaces.
  - `GlowNow.Domain`: Core entities, Value objects, Domain logic.
  - `GlowNow.Infrastructure`: Data access (EF Core), External services implementation.

## Development Standards
- **Warnings as Errors:** All warnings are treated as errors (`Directory.Build.props`).
- **Naming:** PascalCase for public members/types. camelCase for local variables/parameters.
- **Dependency Injection:** Use constructor injection. Register services in `DependencyInjection.cs` in each layer.
- **Endpoints:** Use Minimal API `MapGroup` and `MapGet/Post/etc`. Keep endpoint mappings thin; delegate logic to Application layer (MediatR Handlers).
- **Async:** Always use `async/await` all the way down.
- **Nullability:** Nullable Reference Types are enabled and strictly enforced.

## Commands
```bash
# Run API (from apps/api)
dotnet run --project src/GlowNow.Api

# Build Solution
dotnet build

# Add Migration (if EF Core is used)
dotnet ef migrations add <MigrationName> --project src/GlowNow.Infrastructure --startup-project src/GlowNow.Api
```
