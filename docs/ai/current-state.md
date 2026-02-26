# GlowNow — Current State

> Living document tracking the latest development status. Update this after each meaningful milestone.

**Last updated:** 2026-02-26
**Current branch:** `feat/cognito-auth-migration`
**Latest commit on main:** `7411192` — Merge pull request #7 from ZPLabs/fix/module-unit-of-work
**Latest commit on branch:** `8b9a9d0` — feat(auth): migrate to client-side Cognito auth via AWS Amplify

---

## Project Phase

**Phase: Auth Migration Complete / Pre-MVP**

Client-side Cognito authentication via AWS Amplify is fully implemented and working. Email registration with verification, email login, and Google OAuth login are all functional end-to-end. The API is now a pure JWT consumer — it never handles credentials. Lazy user creation in middleware handles both email and Google-federated users transparently. The mobile app (Expo Router) has been scaffolded with the same auth flows. The project is ready for the next feature module (Clients or Booking).

---

## Recent Work (2026-02-26)

### Cognito Auth Migration — Client-Side Amplify

**Commit:** `8b9a9d0` — "feat(auth): migrate to client-side Cognito auth via AWS Amplify"

**Branch:** `feat/cognito-auth-migration`

#### What Changed

**API:**
- Deleted Login, Logout, RefreshToken commands and all related files
- Deleted `ICognitoIdentityProvider`, `CognitoIdentityProvider`, `AuthTokens`
- Removed `AWSSDK.CognitoIdentityProvider` package
- Rewrote `RegisterBusinessCommand`: removed user fields (email, password, names) — reads authenticated user from `ICurrentUserProvider` instead of creating via Cognito Admin API
- Rewrote `CurrentUserMiddleware`: lazy-creates a local `User` record on the first authenticated request using JWT claims. Handles `given_name`/`family_name` claims, falls back to `name` claim (for Google federation), falls back to email prefix
- Added CORS policy for `http://localhost:3000`
- Stripped `CognitoSettings` to `UserPoolId` + `Region` only (removed `ClientId`, `AccessKey`, `SecretKey`)
- Removed unused `services.Configure<CognitoSettings>` from DI

**Web (`apps/web`):**
- Added `aws-amplify` dependency
- New `src/lib/auth/amplify.ts` — Amplify configuration (user pool, OAuth domain, callbacks)
- Rewrote `AuthContext`: all auth via Amplify SDK (`signIn`, `signUp`, `confirmSignUp`, `signInWithRedirect`, `signOut`), Hub listener for OAuth callback, `UserAlreadyAuthenticatedException` handling
- API client (`client.ts`) now fetches Cognito **ID token** (not access token) from Amplify and attaches as Bearer — ID token contains `email`/`given_name`/`family_name` needed by lazy user creation middleware
- New pages: `/verify-email`, `/register/business`, `/auth/callback`
- New components: `VerifyEmailForm`, `BusinessRegistrationForm`
- Updated `LoginForm`: Google sign-in button, `UserNotConfirmedException` → redirect to verify-email
- Updated `RegisterForm`: removed business fields, wires `phoneNumber` through to Cognito `signUp`
- Deleted `src/lib/auth/tokens.ts` (Amplify manages token storage in cookies)
- Deleted server-side auth API functions (login, register, refreshToken, logout)

**Mobile (`apps/mobile`):**
- Converted to Expo Router structure (deleted `App.tsx`, `index.ts`)
- Added auth screens: login, register, verify-email, register-business, dashboard
- Added `src/contexts/AuthContext.tsx`, `src/lib/auth/amplify.ts`, API client — mirrors web implementation using `AsyncStorage` for token storage

**Docs:**
- Added `docs/AUTH.md` — full explanation of all auth flows, token design, lazy user creation, and key design decisions

---

## Previous Major Work

### Cross-Module UnitOfWork Fix (2026-02-14)

**Commit:** `3e5c581` — "fix(api): resolve cross-module UnitOfWork conflict causing user data loss on registration"

- Fixed `RegisterBusinessCommand` saving to both `IIdentityUnitOfWork` and `IBusinessUnitOfWork` correctly, preventing cross-module data loss when business registration involved multiple DbContexts.

### Login Implementation (2026-02-13)

**Commit:** `c2b46c1` — "feat(web): implement authentication pages with login and register forms"

- Implemented web authentication pages with `LoginForm` and `RegisterForm`
- API-proxied auth flow (pre-migration)

### 4-Project-Per-Module Architecture (2026-02-10)

**Commit:** `2c34bbd` — "refactor(api): restructure to 4-project-per-module architecture with MVC controllers"

- Each module split into Domain, Application, Infrastructure, Api projects
- Minimal APIs converted to MVC Controllers
- SharedKernel / Infrastructure.Core split

---

## What's Done

### Infrastructure

- Turborepo + npm workspaces monorepo
- .NET 10 modular monolith API (4-project-per-module Clean Architecture)
- PostgreSQL via Docker Compose (EF Core migrations generated for Identity, Business, Catalog)
- CORS configured for local development
- JWT Bearer auth against Cognito User Pool
- MediatR pipeline: Logging → Validation → Transaction → Performance behaviors

### Authentication (fully working end-to-end)

- Email registration → verification code → confirm → login
- Email login via Cognito SRP (password never leaves browser)
- Google OAuth login via Cognito federation
- Lazy local user creation on first API call (handles both email and Google users)
- Token auto-refresh managed by Amplify
- Business registration post-auth (owner role assigned)
- Web: all auth pages and flows implemented
- Mobile: Expo Router auth scaffold implemented

### API Modules

| Module | Status | Notes |
|--------|--------|-------|
| Identity | **Complete** | Auth endpoints, user/membership management |
| Business | **Complete** | Business CRUD, operating hours |
| Catalog | **Complete** | Services and categories |
| Team | **Complete** | Staff profiles, scheduling, time-off, availability |
| Clients | Not started | — |
| Booking | Not started | — |
| Notifications | Scaffold only | Event-driven, not wired up |

### Unit Tests — 88+ passing

| Area | Tests |
|------|-------|
| Shared (Entity, AggregateRoot, ValueObject, Result, Error) | 17 |
| Shared Value Objects (Email, PhoneNumber) | 12 |
| Business Value Objects (Ruc) | 7 |
| Business Entities (Business) | 4 |
| Identity Entities (User, BusinessMembership) | 8 |
| Identity Handlers (RegisterBusiness) | 4 |
| Identity Validators (RegisterBusiness) | 4 |
| Team Domain Layer | ~13 |
| **Total** | **69+** |

> Note: Login, Logout, RefreshToken handler/validator tests deleted as part of Cognito migration.

### Docs

| File | Contents |
|------|----------|
| `docs/PRD.md` | Product requirements |
| `docs/ARCHITECTURE.md` | API architecture patterns and conventions |
| `docs/AUTH.md` | Auth flows, token design, lazy user creation |
| `docs/GIT_GUIDE.md` | Branching and commit conventions |
| `apps/api/CLAUDE.md` | API coding conventions for AI |
| `apps/web/CLAUDE.md` | Web coding conventions for AI |

---

## What's Not Done

### Immediate Next Steps

1. **Merge `feat/cognito-auth-migration` → `main`** via PR
2. **Post-login routing** — after login, redirect based on whether user has a business membership (→ `/register/business` if not, → `/dashboard` if yes)
3. **Protected routes** — redirect unauthenticated users away from `/dashboard` and `/register/business`
4. **Clients Module** — client profiles, search, history
5. **Booking Module** — availability engine, appointment creation

### Infrastructure / Cross-Cutting Not Started

- Global exception handling middleware
- Correlation ID middleware
- Rate limiting
- Structured logging (Serilog)
- CI/CD pipeline
- AWS infrastructure (Terraform)
- Production Cognito domain (custom domain vs auto-generated)

### Frontend Not Started

- Dashboard page (placeholder only)
- Post-login routing logic (membership check)
- Protected route guards
- Business settings pages
- Service catalog management UI
- Booking flow UI

---

## Architecture Decisions Log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-01-20 | Modular monolith over microservices | MVP scale doesn't justify distributed complexity |
| 2026-02-07 | `Result<T> : Result` inheritance | Enables pipeline behavior constraints |
| 2026-02-08 | Local User shadow in PostgreSQL | Enables multi-tenancy joins without Cognito limitations |
| 2026-02-09 | OperatingHours as JSONB | Flexible weekly schedule storage |
| 2026-02-09 | Soft deletes for Services and Categories | Preserves booking history integrity |
| 2026-02-10 | 4-project-per-module architecture | Better separation of concerns, cleaner dependency graph, easier testing |
| 2026-02-10 | MVC Controllers over Minimal APIs | Better OpenAPI support, route grouping, familiar patterns |
| 2026-02-10 | SharedKernel/Infrastructure.Core split | Domain primitives isolated from cross-cutting infrastructure |
| 2026-02-26 | Client-side auth via Amplify SDK | Passwords never transit the API; enables Google OAuth and email verification natively |
| 2026-02-26 | ID token sent to API (not access token) | ID token contains email/name claims needed for lazy user creation; access token only has `sub` |
| 2026-02-26 | Lazy user creation in middleware | Single code path for all auth providers; no explicit registration endpoint needed |

---

## API Endpoints Summary

### Authentication (`/api/v1/auth`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/register-business` | Required | Create business + owner membership (user already exists in Cognito) |
| GET | `/me` | Required | Get current user + memberships |

> Login, logout, refresh, and user creation are handled client-side by Amplify SDK. The API no longer has these endpoints.

### Business (`/api/v1/businesses`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/{id}` | Required | Get business details |
| GET | `/{id}/operating-hours` | Required | Get weekly schedule |
| PUT | `/{id}/operating-hours` | Required | Set weekly schedule |
| PUT | `/{id}/settings` | Required | Update name, description, logo |

### Service Categories (`/api/v1/services/categories`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/` | Required | Create category |
| GET | `/?businessId=` | Required | List categories |
| PUT | `/{id}` | Required | Update category |
| DELETE | `/{id}` | Required | Soft-delete category |

### Services (`/api/v1/services`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/` | Required | Create service |
| GET | `/?businessId=` | Required | List services |
| GET | `/{id}` | Required | Get service |
| GET | `/by-category/{categoryId}` | Required | Services by category |
| PUT | `/{id}` | Required | Update service |
| DELETE | `/{id}` | Required | Soft-delete service |

### Staff (`/api/v1/staff`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/` | Required | Create staff profile |
| GET | `/` | Required | List all staff |
| GET | `/{id}` | Required | Get staff profile |
| PUT | `/{id}` | Required | Update staff profile |
| DELETE | `/{id}` | Required | Delete staff profile |
| POST | `/{id}/activate` | Required | Activate staff |
| POST | `/{id}/deactivate` | Required | Deactivate staff |
| PUT | `/{id}/schedule` | Required | Update schedule |
| GET | `/{id}/schedule` | Required | Get schedule |
| GET | `/{id}/availability` | Required | Get availability |
| GET | `/me/schedule` | Required | Get my schedule |
| POST | `/{id}/services` | Required | Assign service |
| DELETE | `/{id}/services/{serviceId}` | Required | Unassign service |
| GET | `/by-service/{serviceId}` | Required | Staff by service |
| POST | `/{id}/time-off` | Required | Request time off |
| GET | `/{id}/time-off` | Required | Get time off |
| POST | `/time-off/{id}/approve` | Required | Approve time off |
| POST | `/time-off/{id}/reject` | Required | Reject time off |
| POST | `/time-off/{id}/cancel` | Required | Cancel time off |
| POST | `/{id}/blocked-times` | Required | Create blocked time |
| GET | `/{id}/blocked-times` | Required | Get blocked times |
| DELETE | `/blocked-times/{id}` | Required | Delete blocked time |
