# Authentication Architecture

## Overview

Cognito owns identity (tokens, passwords, Google OAuth). The API is a pure JWT consumer — it never handles credentials. Amplify SDK runs client-side in the browser and manages the full auth lifecycle.

```
Browser (Amplify SDK) ←→ AWS Cognito ←→ Google OAuth
        ↓
   API (JWT Bearer)
        ↓
   PostgreSQL
```

---

## Cognito Configuration

| Setting | Value |
|---|---|
| User Pool | `sa-east-1_i5sGgrOy8` |
| Region | `sa-east-1` |
| App Client | `glownow-public-client` (no client secret) |
| Auth flows | `ALLOW_USER_SRP_AUTH`, `ALLOW_REFRESH_TOKEN_AUTH`, `ALLOW_USER_AUTH` |
| OAuth grant | Authorization code + PKCE |
| Federated IdP | Google |
| Callback URLs | `http://localhost:3000/auth/callback` |
| Sign-out URLs | `http://localhost:3000/login` |

---

## Flow 1: Email Registration

**Route:** `/register` → `RegisterForm` → `/verify-email` → `VerifyEmailForm`

1. User submits email, password, firstName, lastName, (optional) phoneNumber
2. Amplify `signUp()` → Cognito creates an **unconfirmed** user, sends a 6-digit code to email
3. Browser redirects to `/verify-email?email=...`
4. User submits the code → Amplify `confirmSignUp()` → Cognito marks user as confirmed
5. Browser redirects to `/login?verified=true`

> At this point the user exists in Cognito but **not yet in the local DB**. Local user creation happens lazily on the first authenticated API call.

---

## Flow 2: Email Login

**Route:** `/login` → `LoginForm`

1. User submits email + password
2. Amplify `signIn()` → Cognito SRP auth (password never leaves the browser)
3. Cognito returns ID token + access token + refresh token — Amplify stores them in cookies
4. `AuthContext.checkUser()` → `GET /api/v1/auth/me` (see [Token Flow](#token-flow-every-api-call))
5. On success: `AuthContext` sets `user` state → browser redirects to `/dashboard`

---

## Flow 3: Google Login

**Route:** `/login` → `LoginForm` → "Sign in with Google"

1. Amplify `signInWithRedirect({ provider: 'Google' })` → browser navigates to Cognito hosted UI
2. Cognito redirects to Google OAuth consent screen
3. User approves → Google redirects back to Cognito (`/oauth2/idpresponse`)
4. Cognito exchanges the Google code, maps attributes (`email`, `given_name`, `family_name`), issues its own Cognito tokens, redirects browser to `http://localhost:3000/auth/callback`
5. `/auth/callback` page mounts → Amplify processes the URL params, exchanges the code for tokens, stores them in cookies
6. Amplify fires `signedIn` Hub event → `AuthContext.checkUser()` → `GET /api/v1/auth/me`
7. On success: `AuthContext` sets `user` state → browser redirects to `/dashboard`

**Edge case — user already signed in:**
If `signInWithRedirect` throws `UserAlreadyAuthenticatedException`, `AuthContext` catches it, calls `checkUser()` to refresh state, and `LoginForm` redirects to `/dashboard`.

---

## Flow 4: Business Registration

**Route:** `/register/business` → `BusinessRegistrationForm`

Requires an authenticated user (Cognito + local DB user already exist).

1. User submits businessName, businessRuc (13-digit Ecuador RUC), businessAddress, (optional) phone/email
2. `POST /api/v1/auth/register-business` with ID token in `Authorization` header
3. API `RegisterBusinessCommandHandler`:
   - Validates RUC value object
   - Checks RUC is not already taken
   - Looks up local user via `ICurrentUserProvider` (set by `CurrentUserMiddleware`)
   - Creates `Business` + `BusinessMembership` (role = Owner)
   - Saves to DB
4. `fetchProfile()` re-fetches `GET /me` → user now has a business membership
5. Browser redirects to `/dashboard`

---

## Token Flow (every API call)

```
apiClient()
  → fetchAuthSession()              // reads Cognito ID token from Amplify cookies
  → Authorization: Bearer <id-token>
  → API: JWT Bearer middleware validates signature against Cognito JWKS
  → CurrentUserMiddleware: lookup or lazy-create local user, set user_id claim
  → Controller: reads user_id claim, executes query/command
```

**Why the ID token and not the access token?**
The Cognito access token only contains `sub`. The ID token contains `email`, `given_name`, and `family_name` — required by `CurrentUserMiddleware` for lazy user creation. The API accepts both because `ValidateAudience = false`.

**Token refresh:** Amplify automatically refreshes the ID token using the refresh token before it expires. API calls always get a fresh token transparently.

---

## Lazy User Creation (`CurrentUserMiddleware`)

The API has no registration endpoint. Instead, a local `User` record is created automatically on the **first authenticated API request** from any new Cognito user — whether they signed up via email or Google.

```
Authenticated request arrives
  → Extract sub (cognitoUserId) from JWT
  → Lookup User by cognitoUserId in DB
  → Not found:
      → Extract email from ID token
      → Extract given_name / family_name (falls back to `name` claim, then email prefix)
      → Create User record, save to DB
  → Found (or just created):
      → Set ICurrentUserProvider
      → Add user_id claim to request context
```

This design handles email signup and Google login identically — the API does not know or care which auth method was used.

---

## Key Design Decisions

| Decision | Reason |
|---|---|
| Client-side auth via Amplify | Passwords never transit through the API; enables Google and other social providers natively |
| ID token sent to API | Contains user attributes needed for lazy user creation; access token only has `sub` |
| Lazy user creation in middleware | Single code path for all auth methods; no explicit registration endpoint needed |
| No `/login`, `/logout`, `/refresh` API endpoints | Cognito and Amplify own the full auth lifecycle |
| `[Authorize]` on `/register-business` | User must be Cognito-authenticated before registering a business |
| `ValidateAudience = false` | Accepts both ID tokens (aud = client ID) and access tokens (no aud) |

---

## Relevant Files

| File | Purpose |
|---|---|
| `apps/web/src/lib/auth/amplify.ts` | Amplify configuration (Cognito user pool, OAuth domain, callbacks) |
| `apps/web/src/contexts/AuthContext.tsx` | All client-side auth methods; Hub listener for OAuth callback |
| `apps/web/src/lib/api/client.ts` | API client; fetches ID token from Amplify and attaches as Bearer |
| `apps/web/src/app/auth/callback/page.tsx` | OAuth callback handler; waits for Amplify to process tokens |
| `apps/web/src/app/(auth)/verify-email/page.tsx` | Email verification code entry |
| `apps/web/src/app/(auth)/register/business/page.tsx` | Business registration (post-auth) |
| `apps/api/.../Middleware/CurrentUserMiddleware.cs` | Lazy user creation from JWT claims |
| `apps/api/.../Controllers/AuthController.cs` | `GET /me` and `POST /register-business` endpoints |
| `apps/api/.../Commands/RegisterBusiness/` | Business + membership creation command |
| `apps/api/.../appsettings.Development.json` | Cognito UserPoolId + Region for JWT validation |
