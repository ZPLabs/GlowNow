# CLAUDE.md - Web Context

## Tech Stack
- **Framework:** Next.js 16.1 (App Router)
- **Language:** TypeScript 5.9
- **Library:** React 19
- **Styling:** CSS Modules (`*.module.css`) + Global CSS (`globals.css`).
- **Shared UI:** `@glownow/ui` (Workspace package).

## Development Standards
- **App Router:** Use `app/` directory structure. 
  - `page.tsx`: Route UI.
  - `layout.tsx`: Shared layout.
  - `loading.tsx`: Loading states.
- **Components:** 
  - Default to **Server Components** (`async function`).
  - Use `'use client'` directive at the top of the file only when interactivity (hooks, event listeners) is needed.
- **Data Fetching:** Fetch data directly in Server Components.
- **Linting:** Zero-tolerance policy (`--max-warnings 0`). Run `npm run lint` frequently.

## Commands
```bash
# Start Dev Server (Port 3000)
npm run dev

# Linting
npm run lint

# Type Checking
npm run check-types
```
