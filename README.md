# GlowNow

Multi-tenant SaaS platform for booking and business management in the beauty and wellness industry — built for Ecuador, starting in Cuenca.

## Why GlowNow?

Ecuador's wellness sector lacks a professional, localized booking solution. International platforms don't address local compliance (RUC/SRI), payment methods, or regional needs. GlowNow fills this gap.

## Monorepo Structure

This project is organized as a [Turborepo](https://turborepo.dev) monorepo:

```
apps/
  web/        Next.js 16 — client-facing booking and business dashboard
  api/        .NET 10 — REST API (modular monolith, 4-project-per-module)
    src/
      Api/GlowNow.Api/              # Composition root
      Core/GlowNow.SharedKernel/    # Domain primitives
      Core/GlowNow.Infrastructure.Core/  # Cross-cutting
      Modules/{Module}/             # Domain, Application, Infrastructure, Api
  mobile/     Expo — planned mobile app
packages/
  ui/         Shared React component library (@glownow/ui)
  eslint-config/    Shared ESLint configs
  typescript-config/ Shared TypeScript configs
docs/
  PRD.md      Product Requirements Document
  ARCHITECTURE.md   System architecture
  ai/current-state.md   Development status
```

## Prerequisites

- **Node.js** >= 18 and npm 10.9.2+
- **.NET SDK** 10.0.100+

## Getting Started

```bash
# Install dependencies
npm install

# Start all apps (web on :3000, API on :5249)
npm run dev

# Start a specific app
npx turbo dev --filter=web
npx turbo dev --filter=api
```

## Commands

```bash
npm run build          # Build all apps and packages
npm run lint           # Lint everything (zero warnings enforced)
npm run format         # Prettier on all .ts/.tsx/.md files
npm run check-types    # TypeScript type checking
```

## MVP Scope

The initial release targets:

- **Business onboarding** with RUC validation (Ecuador tax ID)
- **Service catalog** with categories, pricing, duration, and buffer times
- **Team management** with role-based permissions (Owner, Manager, Staff, Receptionist)
- **Shift scheduling** with repeating patterns and time-off
- **Booking engine** with real-time availability, online and walk-in flows
- **Notifications** via email and SMS (confirmation, reminders, cancellations)

See [`docs/PRD.md`](docs/PRD.md) for the full product requirements.

## Team

| Name | Role |
|------|------|
| Mario | Lead Dev — Architecture, .NET API, Next.js, Expo Mobile |
| Juan Pablo (Jimpol) | Infrastructure — AWS, Terraform, Security, CI/CD |
| Patricio (Pato) | PM/QA — Business logic, User Stories, QA |

## License

Private — All rights reserved.
