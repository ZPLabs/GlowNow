# GlowNow — Product Requirements Document

**Version:** 2.0  
**Status:** Active  
**Last Updated:** February 2026

---

## Owners

| Name | Role | Responsibility |
|------|------|----------------|
| Mario | Lead Dev | Architecture, .NET API, Next.js, Expo Mobile |
| Juan Pablo | Infrastructure | AWS, Terraform, Security, CI/CD |
| Patricio | PM/QA | Business logic, User Stories, QA |

---

## 1. Vision & Mission

### What is GlowNow?

GlowNow is a multi-tenant SaaS platform for booking and business management in the beauty and wellness industry — built specifically for Ecuador, starting in Cuenca.

### Why are we building this?

Ecuador's wellness sector lacks a professional, localized booking solution. International platforms like Fresha don't address local compliance (RUC/SRI), payment methods, or regional needs. GlowNow fills this gap.

### MVP Goal

Deliver a functional booking platform within **600–900 hours** that enables:

- Salons to manage services, staff, and appointments
- Clients to discover and book services online
- Automated notifications to reduce no-shows

---

## 2. Success Metrics

| Metric | Target | How We Measure |
|--------|--------|----------------|
| Pilot salons onboarded | 10 | Monthly count |
| Monthly bookings | 500+ | Platform analytics |
| No-show rate | < 15% | Booking completion data |
| Booking completion rate | > 80% | Funnel analytics |
| API response time (p95) | < 200ms | Infrastructure monitoring |

---

## 3. The Ecuador Factor

> These are non-negotiable requirements for operating in Ecuador.

### Compliance
- **Tax ID (RUC/Cédula):** Required for business onboarding (SRI compliance)
- **RUC format:** 13 digits for businesses, 10 for individuals

### Localization
- **Address system:** Support Cuenca parishes (El Sagrario, San Sebastián, etc.)
- **Phone format:** Ecuador mobile (+593 9X XXX XXXX)
- **Language:** Spanish primary, English secondary (expat areas)
- **Currency:** USD (Ecuador's official currency)

---

## 4. User Roles

| Role | Description |
|------|-------------|
| **Platform Admin** | GlowNow staff — manages entire platform |
| **Business Owner** | Salon owner — full access to their business |
| **Manager** | Elevated staff — configurable high access |
| **Staff Member** | Service providers (stylists, therapists) |
| **Receptionist** | Front desk — bookings, no financials |
| **Client** | End customer — books services |

### Permission Matrix (MVP)

| Capability | Owner | Manager | Staff | Receptionist |
|------------|:-----:|:-------:|:-----:|:------------:|
| View own calendar | ✅ | ✅ | ✅ | ✅ |
| View all calendars | ✅ | ✅ | ❌ | ✅ |
| Create/edit appointments | ✅ | ✅ | Own only | ✅ |
| Manage services | ✅ | ✅ | ❌ | ❌ |
| Manage team members | ✅ | ❌ | ❌ | ❌ |
| View reports/financials | ✅ | ✅ | ❌ | ❌ |
| Business settings | ✅ | ❌ | ❌ | ❌ |
| Checkout/payments | ✅ | ✅ | Own clients | ✅ |

---

## 5. Scope

### In Scope (MVP)

**Account & Multi-tenancy**
- Business self-registration with RUC validation
- User authentication (email/password)
- Strict data isolation between businesses

**Service Catalog**
- Services with name, description, duration, price
- Service categories
- Processing time and buffer time
- Assign services to specific staff

**Team Management**
- Add team members with profiles
- Assign permission levels
- Shift scheduling (repeating patterns)
- Time-off and blocked time management

**Booking Engine**
- Real-time availability calculation
- Online booking flow (service → staff → time → confirm)
- Manual booking for walk-ins
- Appointment management (reschedule, cancel, no-show)
- "Any Professional" assignment option

**Notifications**
- Email: Booking confirmation, cancellation, reschedule
- SMS: Confirmation, 24h reminder

**Cancellation Policies**
- Configurable cancellation window
- Cancellation reason tracking
- No-show marking and history

---

### Out of Scope (MVP)

> These are deferred to future phases. Do not add to MVP backlog.

| Feature | Phase | Reason |
|---------|-------|--------|
| Online payment processing | Phase 2 | Regulatory complexity |
| Deposit collection | Phase 2 | Requires payments |
| Marketplace/discovery | Phase 2 | Focus on direct bookings first |
| Multi-location support | Phase 2 | Complexity |
| Recurring appointments | Phase 2 | Complexity |
| Google Calendar sync | Phase 2 | Integration effort |
| Client reviews | Phase 2 | Not core to booking |
| Waitlist | Phase 2 | Not core to booking |
| Loyalty programs | Phase 3 | Not core to booking |
| Marketing campaigns | Phase 3 | Not core to booking |
| Inventory/product sales | Phase 3 | Not core to booking |
| Payroll/commissions | Phase 3 | Not core to booking |
| Gift cards | Phase 3 | Not core to booking |

---

## 6. User Stories Checklist

### Epic 1: Business Onboarding

- [ ] **US-1.1** — Register business with RUC validation
- [ ] **US-1.2** — Set business operating hours
- [ ] **US-1.3** — Upload business logo and description

### Epic 2: Service Management

- [ ] **US-2.1** — Create services with name, duration, price
- [ ] **US-2.2** — Organize services into categories
- [ ] **US-2.3** — Add processing/buffer time to services

### Epic 3: Team Management

- [ ] **US-3.1** — Add team members (sends invitation)
- [ ] **US-3.2** — Assign services to specific staff
- [ ] **US-3.3** — Set permission levels for staff
- [ ] **US-3.4** — Staff can view their own schedule

### Epic 4: Shift Scheduling

- [ ] **US-4.1** — Set repeating shift patterns
- [ ] **US-4.2** — Add time-off for staff
- [ ] **US-4.3** — Block time for meetings/breaks

### Epic 5: Client Booking (Online)

- [ ] **US-5.1** — View available services with pricing
- [ ] **US-5.2** — Select staff or "Any Professional"
- [ ] **US-5.3** — View available time slots
- [ ] **US-5.4** — Complete booking and receive confirmation
- [ ] **US-5.5** — Cancel booking online (within policy)

### Epic 6: Staff Booking (Manual)

- [ ] **US-6.1** — Book walk-in clients
- [ ] **US-6.2** — Search existing clients by phone/name
- [ ] **US-6.3** — Reschedule appointments (drag & drop)
- [ ] **US-6.4** — Mark appointments as completed/no-show

### Epic 7: Notifications

- [ ] **US-7.1** — Send booking confirmation (email + SMS)
- [ ] **US-7.2** — Send 24h reminder (SMS)
- [ ] **US-7.3** — Notify staff of new bookings

### Epic 8: Platform Administration

- [ ] **US-8.1** — View all businesses with RUC
- [ ] **US-8.2** — Suspend business if needed

---

## 7. Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| SMS delivery issues in Ecuador | Medium | High | Research local providers; email fallback |
| Scope creep | High | High | Strict "Out of Scope" enforcement |
| Multi-tenancy data leak | Low | Critical | Row-level security; security audit |
| Calendar calculation bugs | Medium | High | Comprehensive unit tests |
| Pilot salon churn | Medium | Medium | Close feedback loop with pilots |

---

## 8. Dependencies

| Dependency | Owner | Status |
|------------|-------|--------|
| AWS account setup | Jimpol | ⏳ Pending |
| Twilio account (Ecuador SMS) | Jimpol | ⏳ Pending |
| SendGrid/SES account | Jimpol | ⏳ Pending |
| Pilot salon agreements | Pato | ⏳ Pending |
| Domain + SSL certificates | Jimpol | ⏳ Pending |
| App Store / Play Store accounts | Mario | ⏳ Pending |

---

## 9. Timeline

| Phase | Duration | Focus |
|-------|----------|-------|
| Phase 0: Foundation | 2 weeks | Setup, CI/CD, auth, database |
| Phase 1: Business Setup | 3 weeks | Onboarding, services, team, shifts |
| Phase 2: Booking Engine | 4 weeks | Availability, online + manual booking |
| Phase 3: Notifications | 2 weeks | Email/SMS integration |
| Phase 4: Polish & QA | 2 weeks | Bug fixes, performance, pilot testing |
| **MVP Launch** | — | First pilot salons go live |

**Total: ~13 weeks**

---

## 10. Definition of Done

A feature is **DONE** when:

- [ ] Code merged to `main`
- [ ] Unit tests passing
- [ ] Linting passing
- [ ] Deployed to Staging
- [ ] API docs updated (Swagger)
- [ ] QA verified on Web
- [ ] QA verified on Mobile (iOS + Android)
- [ ] No critical bugs open
- [ ] Pato acceptance ✅

---

## Appendix: Fresha Feature Parity

| Fresha Feature | MVP | Phase |
|----------------|:---:|-------|
| Online booking | ✅ | MVP |
| Manual booking | ✅ | MVP |
| Staff scheduling | ✅ | MVP |
| Service catalog | ✅ | MVP |
| Email notifications | ✅ | MVP |
| SMS notifications | ✅ | MVP |
| Multi-location | ❌ | Phase 2 |
| Payment processing | ❌ | Phase 2 |
| Deposits | ❌ | Phase 2 |
| Marketplace | ❌ | Phase 2 |
| Marketing campaigns | ❌ | Phase 3 |
| Loyalty programs | ❌ | Phase 3 |
| Inventory | ❌ | Phase 3 |

---

## Glossary

| Term | Definition |
|------|------------|
| Business/Salon | A tenant on the platform |
| Workspace | The business's environment in GlowNow |
| Walk-in | Client who arrives without booking |
| Buffer Time | Cleanup time between appointments |
| Processing Time | Extra service time (e.g., color setting) |
| No-show | Client who misses their appointment |
| RUC | Ecuador tax ID for businesses (13 digits) |
| Cédula | Ecuador national ID (10 digits) |
