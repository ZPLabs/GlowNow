import { useEffect } from 'react';
import { View, ActivityIndicator, Text } from 'react-native';
import { useRouter } from 'expo-router';
import { Hub } from 'aws-amplify/utils';

export default function AuthCallback() {
  const router = useRouter();

  useEffect(() => {
    const unsubscribe = Hub.listen('auth', ({ payload }) => {
      if (payload.event === 'signedIn') {
        router.replace('/dashboard');
      }
    });

    // Timeout fallback
    const timer = setTimeout(() => {
      router.replace('/dashboard');
    }, 2000);

    return () => {
      unsubscribe();
      clearTimeout(timer);
    };
  }, [router]);

  return (
    <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
      <ActivityIndicator size="large" color="#6366f1" />
      <Text style={{ marginTop: 20, fontSize: 16 }}>Completing sign in...</Text>
    </View>
  );
}
