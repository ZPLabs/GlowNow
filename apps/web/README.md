# GlowNow Web

Next.js 16 web application serving as the primary interface for both clients and business staff.

## Tech Stack

- **Next.js 16** with App Router
- **React 19**
- **CSS Modules** for styling
- **TypeScript 5.9** (strict mode)

## What This App Does

### Client-Facing

- Browse business services with pricing and duration
- Select a staff member or choose "Any Professional"
- View real-time available time slots
- Complete bookings and receive confirmation
- Cancel bookings within the business cancellation policy

### Business Dashboard

- Business registration with RUC validation (Ecuador tax ID)
- Service catalog management (categories, pricing, duration, buffer times)
- Team member management with role-based permissions
- Shift scheduling with repeating patterns and time-off
- Appointment calendar with manual booking for walk-ins
- Reschedule, cancel, or mark appointments as no-show/completed

## Development

```bash
# From the repo root
npx turbo dev --filter=web

# Or from this directory
npm run dev
```

The app runs on [http://localhost:3000](http://localhost:3000).

## Scripts

```bash
npm run dev          # Start dev server on port 3000
npm run build        # Production build
npm run start        # Start production server
npm run lint         # ESLint (zero warnings enforced)
npm run check-types  # TypeScript type checking
```

## Shared Packages

- `@repo/ui` — Shared React component library
- `@repo/eslint-config` — ESLint configuration
- `@repo/typescript-config` — TypeScript configuration
