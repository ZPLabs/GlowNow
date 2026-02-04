# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GlowNow is a multi-tenant SaaS platform for booking and business management in the beauty/wellness industry, targeting Ecuador (Cuenca). It's an early-stage MVP built as a Turborepo monorepo with TypeScript (web) and C#/.NET (API).

## Commands

```bash
# Development (from repo root)
npm run dev                        # Start all apps
npx turbo dev --filter=@glownow/web  # Start only web app (port 3000)
npx turbo dev --filter=@glownow/api  # Start only API (port 5249)

# Build
npm run build                      # Build all apps
npx turbo build --filter=@glownow/web  # Build specific app
npx turbo build --filter=@glownow/api  # Build .NET API

# .NET API (from apps/api)
dotnet build GlowNow.Api.sln      # Build API solution
dotnet run --project src/GlowNow.Api  # Run API directly

# Linting & Formatting
npm run lint                       # Lint all packages (zero warnings enforced)
npm run format                     # Prettier on all .ts/.tsx/.md files
npm run check-types                # TypeScript type checking across all packages
```

## Architecture

**Monorepo layout (Turborepo + npm workspaces):**

- `apps/web` — Next.js 16 web app (React 19, CSS Modules)
- `apps/mobile` — Expo mobile app (React Native, TypeScript)
- `apps/api` — .NET 10 API (Clean Architecture: Api → Application → Domain, Api → Infrastructure → Application → Domain)
- `packages/ui` — Shared React component library (`@glownow/ui`)
- `packages/eslint-config` — Shared ESLint configs (base + Next.js)
- `packages/typescript-config` — Shared TypeScript configs (base, nextjs, react-library)

**Turborepo task pipeline:** Build and lint tasks have `dependsOn: ["^<task>"]`, meaning upstream packages build/lint first. Dev tasks skip caching.

**Package references:** Apps import shared packages via `@glownow/ui`, `@glownow/eslint-config`, `@glownow/typescript-config` workspace references.

## Key Configuration

- **Node >= 18**, npm 10.9.2, TypeScript 5.9.2
- **TypeScript strict mode** enabled across all packages
- **ESLint** uses `eslint-plugin-turbo`, `typescript-eslint`, and `eslint-config-prettier`; the `only-warn` plugin converts errors to warnings, but the web app lint script enforces `--max-warnings 0`
- **No test framework** configured yet

## Product Context

The PRD lives at `docs/PRD.md`. Key planned features: business self-registration with RUC validation, service catalog, team scheduling, real-time availability booking engine, email/SMS notifications, multi-role permissions (Owner, Manager, Staff, Receptionist, Client).
