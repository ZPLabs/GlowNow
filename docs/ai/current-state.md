# GlowNow — Current State

> Living document tracking the latest development status. Update this after each meaningful milestone.

**Last updated:** 2026-02-07
**Current branch:** `feat/shared-module`
**Latest commit:** `b15440a` — feat(api): add GlowNow.Shared foundation layer

---

## Project Phase

**Phase: Foundation / Pre-MVP**

The monorepo scaffold is complete and the shared foundation layer is built. All 7 domain modules exist as empty shells. No module has domain entities, handlers, or infrastructure yet.

---

## What's Done

### Monorepo Scaffold (merged to `main`)

- Turborepo + npm workspaces with `apps/web`, `apps/mobile`, `apps/api`, and shared `packages/`
- Next.js 16 web app (React 19, CSS Modules) — default template, no custom pages
- Expo 54 mobile app — blank TypeScript template, no custom screens
- Shared packages: `@glownow/ui`, `@glownow/eslint-config`, `@glownow/typescript-config`
- .NET 10 API restructured as modular monolith with Clean Architecture
- 7 empty domain modules scaffolded: Identity, Business, Catalog, Team, Clients, Booking, Notifications
- `GET /health` and `GET /health/ready` endpoints
- Docker infrastructure scripts
- ARCHITECTURE.md, PRD.md, GIT_GUIDE.md, CLAUDE.md context files

### GlowNow.Shared Foundation (`feat/shared-module`, not yet merged)

- **Domain Primitives:** `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `ITenantScoped`, `IDomainEvent`
- **Result/Error Pattern:** `Result`, `Result<TValue>`, `Error` (sealed record), `ValidationError`
- **CQRS Abstractions:** `ICommand`/`IQuery` with MediatR, `IBaseCommand` marker, handler interfaces
- **MediatR Pipeline Behaviors** (in order): Logging, Validation, Transaction, Performance
- **Application Interfaces:** `IUnitOfWork`, `IDateTimeProvider`, `ITenantProvider`, `ITransactionManager`
- **Default Implementations:** `SystemDateTimeProvider` (singleton), `NoOpTransactionManager` (scoped, replaceable via `TryAddScoped`)
- **DI Registration:** `AddSharedServices(params Assembly[])` with MediatR assembly scanning, FluentValidation, behavior pipeline
- **Unit Tests:** 17 passing tests covering Entity, AggregateRoot, ValueObject, Result, and Error
- **Build:** 0 warnings, 0 errors

### NuGet Dependencies (Shared)

| Package | Version |
|---------|---------|
| MediatR | 12.4.1 |
| FluentValidation | 11.11.0 |
| FluentValidation.DependencyInjectionExtensions | 11.11.0 |
| Microsoft.Extensions.Logging.Abstractions | 10.0.0-preview.3 |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.0-preview.3 |

---

## What's Not Done

### Immediate Next Steps

1. **Merge `feat/shared-module` to `main`** via PR
2. **Identity Module** — First module to implement (auth is a prerequisite for everything)
   - Domain: User entity, Role enum, domain events (UserRegistered, etc.)
   - Application: Register, Login, RefreshToken commands; GetMe query
   - Infrastructure: EF Core config, JWT token generation, password hashing (bcrypt)
3. **Business Module** — Second priority (multi-tenancy depends on it)
   - Domain: Business entity with RUC validation, OperatingHours value object
   - Application: RegisterBusiness command, GetBusiness query
   - Infrastructure: EF Core config with tenant global query filters

### Not Started (by module)

| Module | Status | Dependencies |
|--------|--------|-------------|
| Identity | Not started | Shared only |
| Business | Not started | Shared only |
| Catalog | Not started | Shared only |
| Team | Not started | Identity, Catalog |
| Clients | Not started | Identity (optional) |
| Booking | Not started | Team, Catalog, Clients |
| Notifications | Not started | Shared only (event-driven) |

### Infrastructure / Cross-Cutting Not Started

- PostgreSQL database setup (local Docker Compose)
- EF Core DbContext and migrations
- Real `ITransactionManager` implementation (EF Core-based, replaces NoOp)
- Real `ITenantProvider` implementation (reads `X-Business-Id` header)
- JWT authentication middleware
- Global exception handling middleware
- Correlation ID middleware
- CORS configuration
- Rate limiting
- Structured logging (Serilog or similar)
- CI/CD pipeline
- AWS infrastructure (Terraform)

### Frontend Not Started

- No custom pages or components in the web app
- No custom screens in the mobile app
- No API client / SDK generation
- No design system beyond the default `@glownow/ui` components (button, card, code)

---

## Architecture Decisions Log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-01-20 | Modular monolith over microservices | MVP scale doesn't justify distributed complexity; can extract modules later |
| 2026-01-20 | Single .csproj per module (not split Domain/Application/Infrastructure) | Keeps it simple at MVP; folder-based separation within each module |
| 2026-02-07 | `Result<T> : Result` inheritance | Enables `where TResponse : Result` constraint in pipeline behaviors for both Result and Result<T> |
| 2026-02-07 | `NoOpTransactionManager` as default | Allows Shared to compile and run without EF Core; Infrastructure overrides via `TryAddScoped` |
| 2026-02-07 | `RaiseDomainEvent` naming (vs `AddDomainEvent`) | Clearer intent — aggregates "raise" events, they don't just "add" them |

---

## Open Questions

- **Auth strategy:** Self-managed JWT or delegate to an identity provider (Auth0, Keycloak)?
- **Database per module or shared DbContext?** Leaning shared for MVP simplicity.
- **API versioning middleware:** Use `Asp.Versioning.Http` or manual route prefixes?
- **Outbox pattern implementation:** Use a library (e.g., MassTransit outbox) or hand-roll with EF Core interceptors?

---

## File Counts

| Area | Source Files | Test Files |
|------|-------------|------------|
| GlowNow.Shared | 21 | — |
| GlowNow.UnitTests/Shared | — | 5 |
| GlowNow.Api | 1 (Program.cs) | — |
| Modules (7, all empty shells) | 7 ({Module}Module.cs each) | — |
| **Total .NET** | **29** | **5** |
