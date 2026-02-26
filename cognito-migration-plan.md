# Plan: Migrate to Client-Side Cognito Authentication

## Context

The API currently proxies all auth through Cognito Admin APIs (AdminCreateUser, AdminInitiateAuth, etc.). This means passwords transit through the API, there's no email verification, no password reset, no MFA, and social login is impossible. Moving auth to the client side via AWS Amplify SDK makes the API a pure JWT consumer, enables Google login and email verification natively, and simplifies the API by removing 4 endpoints and the entire Cognito service layer.

**Decisions:** Amplify SDK with custom forms, Google login now, email verification required, web + mobile.

---

## Phase 1: AWS Cognito Configuration (Manual — AWS Console)

No code changes. Must be done first.

1. **Create new App Client** in User Pool `sa-east-1_i5sGgrOy8`:
   - Name: `glownow-public-client`
   - No client secret (public client)
   - Auth flows: `ALLOW_USER_SRP_AUTH`, `ALLOW_REFRESH_TOKEN_AUTH`, `ALLOW_USER_AUTH`
   - OAuth: Authorization code grant, scopes `openid email profile`
   - Callbacks: `http://localhost:3000/auth/callback`, `glownow://auth/callback`, `exp://127.0.0.1:8081/--/auth/callback`
   - Sign-out URLs: `http://localhost:3000/login`, `glownow://login`

2. **Enable email verification** — verification type: Code (not link)

3. **Add Google as federated IdP:**
   - Create Google OAuth credentials in Google Cloud Console
   - Add Google provider in Cognito, map email/given_name/family_name
   - Configure Cognito domain (e.g. `glownow.auth.sa-east-1.amazoncognito.com`)

4. **Keep old App Client** for rollback safety; decommission after migration verified

---

## Phase 2: API Changes

### 2.1 Delete — Remove server-side Cognito flows

| Delete | Reason |
|--------|--------|
| `src/Modules/Identity/GlowNow.Identity.Application/Commands/Login/` (entire folder) | Login is client-side now |
| `src/Modules/Identity/GlowNow.Identity.Application/Commands/RefreshToken/` (entire folder) | Token refresh is client-side |
| `src/Modules/Identity/GlowNow.Identity.Application/Commands/Logout/` (entire folder) | Logout is client-side |
| `src/Modules/Identity/GlowNow.Identity.Application/Interfaces/ICognitoIdentityProvider.cs` | No more Cognito Admin API calls |
| `src/Modules/Identity/GlowNow.Identity.Application/Interfaces/AuthTokens.cs` | Tokens handled client-side |
| `src/Modules/Identity/GlowNow.Identity.Infrastructure/Services/CognitoIdentityProvider.cs` | Entire service removed |

### 2.2 Modify — AuthController

**File:** `src/Modules/Identity/GlowNow.Identity.Api/Controllers/AuthController.cs`

- **Remove** endpoints: `POST /login`, `POST /refresh`, `POST /logout`
- **Modify** `POST /register` → `POST /register-business` with `[Authorize]` (user must already be authenticated via Cognito)
- **Keep** `GET /me` unchanged

### 2.3 Modify — RegisterBusinessCommand + Handler

**File:** `src/Modules/Identity/GlowNow.Identity.Application/Commands/RegisterBusiness/RegisterBusinessCommand.cs`

Remove `Email`, `Password`, `FirstName`, `LastName`, `PhoneNumber` from the command. These now come from the JWT (via `ICurrentUserProvider`). Command keeps only business fields: `BusinessName`, `BusinessRuc`, `BusinessAddress`, `BusinessPhoneNumber?`, `BusinessEmail?`.

**File:** `src/Modules/Identity/GlowNow.Identity.Application/Commands/RegisterBusiness/RegisterBusinessCommandHandler.cs`

Major rewrite:
- Remove `ICognitoIdentityProvider` dependency
- Add `ICurrentUserProvider` dependency to get the authenticated user
- Get existing local User (created by lazy creation in middleware)
- Create only Business + Membership (no Cognito user creation)
- Remove compensation logic (no Cognito user to roll back)
- Still saves both `IIdentityUnitOfWork` and `IBusinessUnitOfWork`

**File:** `src/Modules/Identity/GlowNow.Identity.Application/Commands/RegisterBusiness/RegisterBusinessCommandValidator.cs`

Remove email/password/name validations. Keep only business field validations.

### 2.4 Modify — CurrentUserMiddleware (lazy user creation)

**File:** `src/Api/GlowNow.Api/Middleware/CurrentUserMiddleware.cs`

This is the most critical change. When an authenticated JWT arrives but no local user exists, auto-create the User from JWT claims (`sub`, `email`, `given_name`, `family_name`). This handles:
- First request after client-side signup
- First Google login (user never existed locally)

Add dependencies to `InvokeAsync`: `IIdentityUnitOfWork`, `IDateTimeProvider`, `ILogger<CurrentUserMiddleware>`

Logic:
```
if authenticated && cognitoUserId not empty:
  user = lookup by cognitoUserId
  if user is null:
    extract email, firstName, lastName from JWT claims
    create User via User.Create(email, firstName, lastName, null, cognitoUserId, now)
    save via IIdentityUnitOfWork
  if user is not null:
    set ICurrentUserProvider and add user_id claim
```

### 2.5 Modify — Infrastructure DI

**File:** `src/Modules/Identity/GlowNow.Identity.Infrastructure/DependencyInjection.cs`

Remove: `IAmazonCognitoIdentityProvider` registration, `ICognitoIdentityProvider` registration, `CognitoSettings` Configure call. Keep: DbContext, UnitOfWork, repository registrations.

**File:** `src/Modules/Identity/GlowNow.Identity.Infrastructure/GlowNow.Identity.Infrastructure.csproj`

Remove `AWSSDK.CognitoIdentityProvider` package reference.

### 2.6 Modify — CognitoSettings

**File:** `src/Modules/Identity/GlowNow.Identity.Application/Interfaces/CognitoSettings.cs`

Remove `ClientId`, `AccessKey`, `SecretKey`. Keep `UserPoolId`, `Region` (still needed by JWT validation in Program.cs).

### 2.7 Modify — Program.cs

**File:** `src/Api/GlowNow.Api/Program.cs`

- Add CORS configuration for `http://localhost:3000` (web dev) and production domain
- JWT config stays as-is (already validates Cognito tokens correctly)

### 2.8 Modify — appsettings

**File:** `src/Api/GlowNow.Api/appsettings.json`

Remove `AccessKey`, `SecretKey`, `ClientId` from Cognito section. Keep `UserPoolId`, `Region`.

### 2.9 Tests

**File:** `tests/GlowNow.UnitTests/Identity/Application/Commands/RegisterBusiness/RegisterBusinessCommandHandlerTests.cs`

Rewrite: remove `ICognitoIdentityProvider` mock, add `ICurrentUserProvider` mock, update command shape (no email/password), remove Cognito compensation tests.

---

## Phase 3: Web Changes (Next.js 16)

### 3.1 Dependencies

Add `aws-amplify` to `apps/web/package.json`.

### 3.2 New files

| File | Purpose |
|------|---------|
| `src/lib/auth/amplify.ts` | Amplify.configure() with Cognito settings from env vars |
| `src/app/(auth)/verify-email/page.tsx` | Email verification code input page |
| `src/app/(auth)/register/business/page.tsx` | Business registration form (post-auth) |
| `src/app/auth/callback/page.tsx` | OAuth callback handler for Google redirect |
| `src/components/auth/VerifyEmailForm/` | Verification code form component |
| `src/components/auth/BusinessRegistrationForm/` | Business fields form component |
| `.env.example` | Template with all env vars (committed, no values) |

### 3.3 Delete

| File | Reason |
|------|--------|
| `src/lib/auth/tokens.ts` | Amplify manages token storage |

### 3.4 Modify — AuthContext.tsx (complete rewrite)

Replace all API-based auth with Amplify SDK calls:
- `signInWithEmail(email, password)` → calls `signIn` from `aws-amplify/auth`
- `signUpWithEmail(email, password, firstName, lastName)` → calls `signUp`
- `confirmSignUpCode(email, code)` → calls `confirmSignUp`
- `signInWithGoogle()` → calls `signInWithRedirect({ provider: 'Google' })`
- `logout()` → calls `signOut`
- `registerBusiness(data)` → calls API `POST /register-business`

Listen to Amplify Hub auth events for Google redirect callback. Use `fetchAuthSession()` for token access (auto-refresh handled by Amplify).

On init: call `getCurrentUser()` from Amplify, then fetch profile from API `/me`.

### 3.5 Modify — API client

**File:** `src/lib/api/client.ts`

Get access token from `fetchAuthSession()` instead of accepting it as parameter. Keep fetch wrapper structure.

### 3.6 Modify — API auth functions

**File:** `src/lib/api/auth.ts`

Remove `login()`, `register()`, `refreshToken()`, `logout()`. Keep `getCurrentUser()`. Add `registerBusiness()`.

### 3.7 Modify — Types

**File:** `src/types/auth.ts`

Remove `LoginRequest`, `LoginResponse`, `RegisterRequest`, `RegisterResponse`, `RefreshTokenRequest`, `RefreshTokenResponse`. Add `RegisterBusinessRequest`, `RegisterBusinessResponse`. Keep `User`, `UserMembership`, `AuthState`.

### 3.8 Modify — LoginForm

**File:** `src/components/auth/LoginForm/LoginForm.tsx`

- Call `signInWithEmail` instead of `login`
- Add "Sign in with Google" button calling `signInWithGoogle`
- Handle `UserNotConfirmedException` → redirect to `/verify-email`

### 3.9 Modify — RegisterForm

**File:** `src/components/auth/RegisterForm/RegisterForm.tsx`

- Remove business fields (move to separate BusinessRegistrationForm)
- Call `signUpWithEmail` instead of `register`
- On success, redirect to `/verify-email?email=...`

### 3.10 Modify — Root layout

**File:** `src/app/layout.tsx`

Add `import "@/lib/auth/amplify"` before AuthProvider.

### 3.11 Environment

**File:** `.env.local` (not committed)
```
NEXT_PUBLIC_API_URL=http://localhost:5249
NEXT_PUBLIC_COGNITO_USER_POOL_ID=sa-east-1_i5sGgrOy8
NEXT_PUBLIC_COGNITO_CLIENT_ID=<new-public-client-id>
NEXT_PUBLIC_COGNITO_DOMAIN=glownow.auth.sa-east-1.amazoncognito.com
NEXT_PUBLIC_AUTH_REDIRECT_SIGN_IN=http://localhost:3000/auth/callback
NEXT_PUBLIC_AUTH_REDIRECT_SIGN_OUT=http://localhost:3000/login
```

---

## Phase 4: Mobile Implementation (Expo)

The mobile app is a blank Expo project. We build auth from scratch.

### 4.1 Dependencies

```
expo-router expo-linking expo-constants expo-web-browser
aws-amplify @aws-amplify/react-native
@react-native-async-storage/async-storage
react-native-get-random-values react-native-url-polyfill
```

### 4.2 Convert to Expo Router

- Modify `app.json`: add `"scheme": "glownow"`, add `expo-router` plugin
- Modify `package.json`: set `"main": "expo-router/entry"`
- Delete `App.tsx` (replaced by `app/` directory)

### 4.3 File structure

```
apps/mobile/
├── app/
│   ├── _layout.tsx              # Root layout (Amplify config + AuthProvider)
│   ├── (auth)/
│   │   ├── _layout.tsx          # Auth group layout
│   │   ├── login.tsx            # Login screen
│   │   ├── register.tsx         # Register screen (account only)
│   │   ├── verify-email.tsx     # Email verification screen
│   │   └── register-business.tsx # Business registration screen
│   ├── (app)/
│   │   ├── _layout.tsx          # Protected layout (checks auth)
│   │   └── dashboard.tsx        # Dashboard screen
│   └── auth/
│       └── callback.tsx         # OAuth callback handler
├── src/
│   ├── contexts/AuthContext.tsx  # Same pattern as web
│   ├── hooks/useAuth.ts
│   ├── lib/auth/amplify.ts      # Amplify config (AsyncStorage for tokens)
│   ├── lib/api/client.ts        # API client
│   ├── lib/api/auth.ts          # getCurrentUser, registerBusiness
│   ├── types/auth.ts            # Shared types
│   └── components/ui/           # Basic Button, Input, Alert components
```

### 4.4 Amplify config

Same as web but using `EXPO_PUBLIC_*` env vars and configuring `AsyncStorage` as the token storage provider via `cognitoUserPoolsTokenProvider.setKeyValueStorage(AsyncStorage)`.

### 4.5 Auth flow

Identical to web: Amplify handles signIn/signUp/confirmSignUp/signOut. API client calls `/me` and `/register-business`. Google login via `signInWithRedirect` opens system browser.

---

## Phase 5: Implementation Order

1. **Cognito console config** (Phase 1) — prerequisite, no code
2. **API changes** (Phase 2) — deploy first, backward compatible if old endpoints kept temporarily
3. **Web changes** (Phase 3) — deploy with API
4. **Mobile** (Phase 4) — can be parallel with web
5. **Cleanup** — remove old App Client, rotate AWS keys

---

## Verification

1. **Email signup flow:** register → verify code → sign in → register business → dashboard
2. **Google login flow:** click Google → authorize → callback → lazy user creation → register business → dashboard
3. **Return visit:** page load → Amplify restores session → API /me → dashboard
4. **Token refresh:** wait for expiry → Amplify auto-refreshes → API calls work
5. **Logout:** sign out → tokens cleared → redirected to login
6. **API build:** `dotnet build GlowNow.Api.sln` — 0 errors, 0 warnings
7. **API tests:** `dotnet test` — all pass (updated RegisterBusiness tests, new middleware tests)
8. **Mobile:** same flows on iOS/Android simulator
