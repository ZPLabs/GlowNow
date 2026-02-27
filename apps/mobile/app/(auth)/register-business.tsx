import React, { useState } from 'react';
import { View, Text, StyleSheet, TextInput, TouchableOpacity, ActivityIndicator, ScrollView } from 'react-native';
import { useAuth } from '../../src/hooks/useAuth';
import { useRouter } from 'expo-router';

export default function RegisterBusiness() {
  const [name, setName] = useState('');
  const [ruc, setRuc] = useState('');
  const [address, setAddress] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { registerBusiness } = useAuth();
  const router = useRouter();

  const handleRegister = async () => {
    if (!name || !ruc || !address) return;
    
    setIsSubmitting(true);
    setError(null);
    try {
      await registerBusiness({
        businessName: name,
        businessRuc: ruc,
        businessAddress: address,
      });
      router.replace('/dashboard');
    } catch (err: any) {
      setError(err.message || 'Business registration failed');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Text style={styles.title}>Register Business</Text>
      <Text style={styles.subtitle}>Set up your business profile</Text>

      {error && <Text style={styles.errorText}>{error}</Text>}

      <TextInput
        style={styles.input}
        placeholder="Business Name"
        value={name}
        onChangeText={setName}
      />

      <TextInput
        style={styles.input}
        placeholder="RUC (13 digits)"
        value={ruc}
        onChangeText={setRuc}
        keyboardType="number-pad"
        maxLength={13}
      />

      <TextInput
        style={styles.input}
        placeholder="Business Address"
        value={address}
        onChangeText={setAddress}
        multiline
      />

      <TouchableOpacity 
        style={styles.button} 
        onPress={handleRegister}
        disabled={isSubmitting}
      >
        {isSubmitting ? (
          <ActivityIndicator color="white" />
        ) : (
          <Text style={styles.buttonText}>Complete Setup</Text>
        )}
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flexGrow: 1,
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
});
