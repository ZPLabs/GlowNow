# CLAUDE.md - Mobile Context

## Tech Stack
- **Framework:** React Native 0.81 / Expo 54
- **Language:** TypeScript 5.9
- **UI:** React Native core components.
- **State Management:** (Check if Context or external lib is used)
- **Navigation:** Expo Router (if `app/` exists) or React Navigation.

## Development Standards
- **Components:** Functional components with Hooks.
- **Styling:** Use `StyleSheet.create` or inline styles sparingly. 
- **Platform Specifics:** Handle iOS/Android differences using `Platform.OS` or `.ios.js`/`.android.js` extensions.
- **Images:** Use `@2x` and `@3x` suffixes for assets if not using SVGs.

## Commands
```bash
# Start Development Server
npm run start 
# or 
npx expo start

# Run on Android Emulator
npm run android

# Run on iOS Simulator (macOS only)
npm run ios
```
