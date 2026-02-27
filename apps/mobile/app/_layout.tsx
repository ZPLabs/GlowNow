import { Slot } from 'expo-router';
import { AuthProvider } from '../src/contexts/AuthContext';
import '../src/lib/auth/amplify';
import { SafeAreaProvider } from 'react-native-safe-area-context';

export default function RootLayout() {
  return (
    <SafeAreaProvider>
      <AuthProvider>
        <Slot />
      </AuthProvider>
    </SafeAreaProvider>
  );
}
