# GlowNow — Current State

> Living document tracking the latest development status. Update this after each meaningful milestone.

**Last updated:** 2026-02-08
**Current branch:** `feat(api)/identity-module-implementation`
**Latest commit on main:** `252cb11` — Merge pull request #2 from ZPLabs/feat/shared-module

---

## Project Phase

**Phase: Identity Module / Pre-MVP**

The monorepo scaffold is complete, the shared foundation layer is merged to `main`, and the Identity module implementation is in progress on a feature branch. Identity domain, application, and infrastructure layers are fully implemented along with partial Business module support (enough for registration). 88 unit tests pass. No database migrations have been applied yet.

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

### GlowNow.Shared Foundation (merged to `main`)

- **Domain Primitives:** `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `ITenantScoped`, `IDomainEvent`
- **Result/Error Pattern:** `Result`, `Result<TValue>`, `Error` (sealed record), `ValidationError`
- **CQRS Abstractions:** `ICommand`/`IQuery` with MediatR, `IBaseCommand` marker, handler interfaces
- **MediatR Pipeline Behaviors** (in order): Logging, Validation, Transaction, Performance
- **Application Interfaces:** `IUnitOfWork`, `IDateTimeProvider`, `ITenantProvider`, `ITransactionManager`
- **Default Implementations:** `SystemDateTimeProvider` (singleton), `NoOpTransactionManager` (scoped, replaceable via `TryAddScoped`)
- **DI Registration:** `AddSharedServices(params Assembly[])` with MediatR assembly scanning, FluentValidation, behavior pipeline

### Shared Infrastructure (`feat(api)/identity-module-implementation`, not yet merged)

- **EF Core Integration:** `AppDbContext` with multi-module entity configuration scanning, `EfCoreTransactionManager` (replaces NoOp)
- **Value Objects:** `Email` (format validation, lowercase normalization, max 256 chars), `PhoneNumber` (Ecuador +593 format, normalizes spaces/dashes/local formats)
- **Cross-Cutting Services:** `ICurrentUserProvider` (scoped, set by middleware), `HttpTenantProvider` (reads `X-Business-Id` header)
- **DI Registration:** `AddSharedInfrastructure(IConfiguration, params Assembly[])` registers DbContext, UnitOfWork, TransactionManager

### Identity Module (`feat(api)/identity-module-implementation`, not yet merged)

- **Domain Layer:**
  - `User` aggregate root — Email, FirstName, LastName, PhoneNumber?, CognitoUserId, timestamps, Memberships collection
  - `BusinessMembership` entity — UserId, BusinessId, Role, CreatedAtUtc
  - `UserRole` enum — Owner, Manager, Staff, Receptionist, Client
  - `UserRegisteredEvent` domain event
  - `IdentityErrors` — EmailAlreadyInUse, InvalidCredentials, UserNotFound, CognitoError, InvalidRefreshToken

- **Application Layer (CQRS):**
  - `RegisterBusinessCommand` + Handler + Validator — Creates Cognito user + local User + Business + BusinessMembership (Owner role). Compensating Cognito delete on DB failure.
  - `LoginCommand` + Handler + Validator — Authenticates via Cognito, looks up local User for memberships
  - `LogoutCommand` + Handler + Validator — Global sign-out via Cognito
  - `RefreshTokenCommand` + Handler + Validator — Token refresh via Cognito
  - `GetCurrentUserQuery` + Handler — Loads User + memberships with business names

- **Application Interfaces:**
  - `ICognitoIdentityProvider` — RegisterUser, Login, RefreshToken, GlobalSignOut, DeleteUser
  - `IUserRepository`, `IBusinessMembershipRepository`, `IBusinessRepository`
  - `AuthTokens` record, `CognitoSettings` options class

- **Infrastructure Layer:**
  - EF Core configurations for User and BusinessMembership (Fluent API, value object mapping via OwnsOne)
  - Repository implementations: UserRepository, BusinessMembershipRepository, BusinessRepository
  - `CognitoIdentityProvider` — AWS SDK adapter mapping Cognito exceptions to domain errors

### Business Module (Partial — `feat(api)/identity-module-implementation`, not yet merged)

- **Domain Layer:**
  - `Business` aggregate root — Name, Ruc, Address, PhoneNumber?, Email, BusinessId (= own Id for ITenantScoped)
  - `Ruc` value object — Validates 10-digit cedula or 13-digit RUC, province code 01-24 or 30
  - `BusinessRegisteredEvent` domain event
  - `BusinessErrors` — DuplicateRuc, BusinessNotFound, InvalidRuc
- **Infrastructure:** EF Core `BusinessConfiguration` (value object column mapping)

### API Host Wiring (`feat(api)/identity-module-implementation`, not yet merged)

- JWT Bearer authentication (Cognito JWKS endpoint)
- `CurrentUserMiddleware` — reads JWT claims, sets `ICurrentUserProvider`
- `TenantMiddleware` — reads `X-Business-Id` header, validates membership
- `AuthEndpoints` — POST register/login/refresh/logout, GET me
- `ResultExtensions` + `ApiResponse` — standard response envelope
- Cognito + connection string configuration in appsettings

### Unit Tests — 88 passing (0 failures, 0 warnings)

| Area | Test Files | Tests |
|------|-----------|-------|
| Shared (Entity, AggregateRoot, ValueObject, Result, Error) | 5 | 17 |
| Shared Value Objects (Email, PhoneNumber) | 2 | 12 |
| Business Value Objects (Ruc) | 1 | 7 |
| Business Entities (Business) | 1 | 4 |
| Identity Entities (User, BusinessMembership) | 2 | 8 |
| Identity Handlers (RegisterBusiness, Login, Logout, RefreshToken, GetCurrentUser) | 5 | 14 |
| Identity Validators (RegisterBusiness, Login, Logout, RefreshToken) | 4 | 13 |
| **Subtotals** | **20** | **75** |
| Shared foundation (pre-existing) | — | 13 |
| **Total** | **20** | **88** |

### NuGet Dependencies

| Package | Version | Project |
|---------|---------|---------|
| MediatR | 12.4.1 | Shared |
| FluentValidation | 11.11.0 | Shared |
| FluentValidation.DependencyInjectionExtensions | 11.11.0 | Shared |
| Microsoft.Extensions.Logging.Abstractions | 10.0.0-preview.3 | Shared |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.0-preview.3 | Shared |
| Microsoft.EntityFrameworkCore | 10.0.0-preview.3 | Shared |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.0-preview.3 | Shared |
| Microsoft.Extensions.Configuration.Abstractions | 10.0.0-preview.3 | Shared |
| AWSSDK.CognitoIdentityProvider | latest | Identity |
| Microsoft.Extensions.Options.ConfigurationExtensions | 10.0.0-preview.3 | Identity |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.0-preview.3 | Api |
| Microsoft.EntityFrameworkCore.Design | 10.0.0-preview.3 | Api |
| xUnit | 2.9.3 | UnitTests |
| NSubstitute | 5.3.0 | UnitTests |
| FluentAssertions | 8.2.0 | UnitTests |

---

## What's Not Done

### Immediate Next Steps

1. **Merge `feat(api)/identity-module-implementation` to `main`** via PR
2. **Run EF Core migrations** — `dotnet ef migrations add InitialCreate` to generate users, businesses, business_memberships tables
3. **Start PostgreSQL** via Docker Compose and apply migration
4. **Smoke test with real Cognito User Pool** — requires configuring `appsettings.Development.json` with real pool credentials
5. **Business Module completion** — Operating hours, settings, full CRUD
6. **Catalog Module** — Services, categories, pricing

### Not Started (by module)

| Module | Status | Dependencies |
|--------|--------|-------------|
| Identity | **Implemented** (domain + app + infra + tests) | Shared, Business (partial) |
| Business | **Partial** (domain + EF config only, no handlers/queries) | Shared only |
| Catalog | Not started | Shared only |
| Team | Not started | Identity, Catalog |
| Clients | Not started | Identity (optional) |
| Booking | Not started | Team, Catalog, Clients |
| Notifications | Not started | Shared only (event-driven) |

### Infrastructure / Cross-Cutting Not Started

- PostgreSQL database setup (local Docker Compose) — config exists, not applied
- EF Core migrations — not yet generated
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
| 2026-02-08 | AWS Cognito for authentication | API-proxied `ADMIN_NO_SRP_AUTH` flow — server holds Cognito credentials, clients never interact with Cognito directly |
| 2026-02-08 | Local User shadow in PostgreSQL | Enables multi-tenancy joins, audit logging, role management without Cognito custom attributes limitations |
| 2026-02-08 | Compensating Cognito delete on DB failure | If local DB save fails after Cognito user creation, delete the Cognito user to avoid orphans |
| 2026-02-08 | Shared value objects (Email, PhoneNumber) | Reused across Identity, Business, Clients, Team modules |
| 2026-02-08 | Ruc in Business module | Ecuador-specific, only relevant to Business domain |
| 2026-02-08 | `IBusinessRepository` in Identity module | Only needs `ExistsByRucAsync` + `Add` + `GetByIdAsync` for registration. Business module will own full repository later |

---

## Open Questions

- **Database per module or shared DbContext?** Using shared `AppDbContext` for MVP — revisit if modules need isolation.
- **API versioning middleware:** Use `Asp.Versioning.Http` or manual route prefixes?
- **Outbox pattern implementation:** Use a library (e.g., MassTransit outbox) or hand-roll with EF Core interceptors?
- **Cognito User Pool setup:** Need to configure a real pool for local development testing.

---

## File Counts

| Area | Source Files | Test Files | Tests |
|------|-------------|------------|-------|
| GlowNow.Shared | 33 | — | — |
| GlowNow.Api | 6 | — | — |
| GlowNow.Identity | 37 | — | — |
| GlowNow.Business | 7 | — | — |
| Modules (5 empty shells) | 5 | — | — |
| GlowNow.UnitTests (GlobalUsings) | — | 1 | — |
| GlowNow.UnitTests/Shared | — | 7 | 30 |
| GlowNow.UnitTests/Identity | — | 11 | 35 |
| GlowNow.UnitTests/Business | — | 2 | 11 |
| **Total .NET** | **88** | **21** | **88** |
