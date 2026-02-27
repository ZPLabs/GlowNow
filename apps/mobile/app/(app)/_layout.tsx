import { Redirect, Stack } from 'expo-router';
import { useAuth } from '../../src/hooks/useAuth';
import { ActivityIndicator, View } from 'react-native';

export default function AppLayout() {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
        <ActivityIndicator size="large" color="#6366f1" />
      </View>
    );
  }

  if (!isAuthenticated) {
    return <Redirect href="/login" />;
  }

  return <Stack />;
}
