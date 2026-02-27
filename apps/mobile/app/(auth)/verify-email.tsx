import React, { useState } from 'react';
import { View, Text, StyleSheet, TextInput, TouchableOpacity, ActivityIndicator } from 'react-native';
import { useAuth } from '../../src/hooks/useAuth';
import { useRouter, useLocalSearchParams } from 'expo-router';

export default function VerifyEmail() {
  const { email } = useLocalSearchParams<{ email: string }>();
  const [code, setCode] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { confirmSignUpCode } = useAuth();
  const router = useRouter();

  const handleVerify = async () => {
    if (!code || !email) return;
    
    setIsSubmitting(true);
    setError(null);
    try {
      await confirmSignUpCode(email, code);
      router.replace('/login');
    } catch (err: any) {
      setError(err.message || 'Verification failed');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Verify Email</Text>
      <Text style={styles.subtitle}>Enter the code sent to {email}</Text>

      {error && <Text style={styles.errorText}>{error}</Text>}

      <TextInput
        style={styles.input}
        placeholder="Verification Code"
        value={code}
        onChangeText={setCode}
        keyboardType="number-pad"
        maxLength={6}
      />

      <TouchableOpacity 
        style={styles.button} 
        onPress={handleVerify}
        disabled={isSubmitting}
      >
        {isSubmitting ? (
          <ActivityIndicator color="white" />
        ) : (
          <Text style={styles.buttonText}>Verify</Text>
        )}
      </TouchableOpacity>

      <TouchableOpacity onPress={() => router.back()}>
        <Text style={styles.linkText}>Back to Registration</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 20,
    justifyContent: 'center',
    backgroundColor: 'white',
  },
  title: {
    fontSize: 32,
    fontWeight: 'bold',
    textAlign: 'center',
    marginBottom: 10,
  },
  subtitle: {
    fontSize: 16,
    color: '#666',
    textAlign: 'center',
    marginBottom: 40,
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    padding: 15,
    borderRadius: 10,
    marginBottom: 15,
    fontSize: 16,
    textAlign: 'center',
    letterSpacing: 5,
  },
  button: {
    backgroundColor: '#6366f1',
    padding: 15,
    borderRadius: 10,
    alignItems: 'center',
    marginTop: 10,
  },
  buttonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold',
  },
  errorText: {
    color: 'red',
    marginBottom: 20,
    textAlign: 'center',
  },
  linkText: {
    color: '#6366f1',
    textAlign: 'center',
    marginTop: 20,
    fontSize: 14,
  },
});
