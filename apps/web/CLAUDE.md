# CLAUDE.md — Web App Context

## 🎯 Project Overview
- **Role:** Client-facing booking portal for GlowNow (beauty/wellness SaaS)
- **Stack:** Next.js 16.1 (App Router), React 19, TypeScript 5.9
- **Styling:** CSS Modules (`*.module.css`) + Global CSS (`globals.css`)
- **Shared UI:** `@glownow/ui` workspace package

## 🛠️ Critical Commands
```bash
# Development (Port 3000)
npm run dev

# Build
npm run build

# Linting (zero-tolerance: --max-warnings 0)
npm run lint

# Type Checking
npm run check-types
```

## 📏 Coding Standards
- **Components:** Default to **Server Components**. Only add `'use client'` when interactivity (hooks, events) is required.
- **Naming:** Use `PascalCase` for components, `camelCase` for variables/functions.
- **Data Fetching:** Fetch directly in Server Components; avoid client-side fetching unless necessary.
- **Styling:** One `.module.css` file per component; use `styles.className` imports.
- **Imports:** Use `@glownow/ui` for shared components; avoid duplicating UI code.

## 📁 App Router Structure
- `app/page.tsx` — Route UI
- `app/layout.tsx` — Root layout (fonts, metadata)
- `app/loading.tsx` — Suspense loading states
- `app/error.tsx` — Error boundaries
- `app/globals.css` — Global styles

## ⚠️ Gotchas & Warnings
- **IMPORTANT:** Linting is zero-tolerance (`--max-warnings 0`). Run `npm run lint` before committing.
- **Type Generation:** Run `next typegen` (via `check-types`) after modifying route params or metadata.
- **React 19:** Uses new features (use hook, Actions). Check compatibility before adding libraries.
- **Turbo Cache:** If builds seem stale, run `npx turbo clean` from repo root.

## 🔗 Extended Memory
- Root project context: @CLAUDE.md (repo root)
- Product requirements: @docs/PRD.md
- Shared UI components: @packages/ui
