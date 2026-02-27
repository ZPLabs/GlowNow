"use client";

import "@/lib/auth/amplify";
import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { 
  signIn, 
  signUp, 
  confirmSignUp, 
  signOut, 
  getCurrentUser as getAmplifyUser,
  fetchAuthSession,
  signInWithRedirect
} from 'aws-amplify/auth';
import { Hub } from 'aws-amplify/utils';
import type { User, AuthState, RegisterBusinessRequest } from "@/types/auth";
import * as authApi from "@/lib/api/auth";

export interface AuthContextValue extends AuthState {
  signInWithEmail: (email: string, password: string) => Promise<void>;
  signUpWithEmail: (email: string, password: string, firstName: string, lastName: string, phoneNumber?: string) => Promise<void>;
  confirmSignUpCode: (email: string, code: string) => Promise<void>;
  signInWithGoogle: () => Promise<void>;
  logout: () => Promise<void>;
  registerBusiness: (data: RegisterBusinessRequest) => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const isAuthenticated = !!user;

  const fetchProfile = useCallback(async () => {
    try {
      const profile = await authApi.getCurrentUser();
      setUser(profile);
    } catch (error) {
      console.error('Failed to fetch user profile', error);
      setUser(null);
    }
  }, []);

  const checkUser = useCallback(async () => {
    try {
      await getAmplifyUser();
      await fetchProfile();
    } catch {
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  }, [fetchProfile]);

  useEffect(() => {
    checkUser();

    const unsubscribe = Hub.listen('auth', ({ payload }) => {
      switch (payload.event) {
        case 'signedIn':
          checkUser();
          break;
        case 'signedOut':
          setUser(null);
          break;
      }
    });

    return () => unsubscribe();
  }, [checkUser]);

  const signInWithEmail = useCallback(async (email: string, password: string) => {
    await signIn({ username: email, password });
    await checkUser();
  }, [checkUser]);

  const signUpWithEmail = useCallback(async (email: string, password: string, firstName: string, lastName: string, phoneNumber?: string) => {
    await signUp({
      username: email,
      password,
      options: {
        userAttributes: {
          email,
          given_name: firstName,
          family_name: lastName,
          ...(phoneNumber ? { phone_number: phoneNumber } : {}),
        }
      }
    });
  }, []);

  const confirmSignUpCode = useCallback(async (email: string, code: string) => {
    await confirmSignUp({ username: email, confirmationCode: code });
  }, []);

  const signInWithGoogle = useCallback(async () => {
    try {
      await signInWithRedirect({ provider: 'Google' });
    } catch (err: unknown) {
      if (err instanceof Error && err.name === 'UserAlreadyAuthenticatedException') {
        // Session already exists — refresh auth state instead of starting a new OAuth flow
        await checkUser();
        return;
      }
      throw err;
    }
  }, [checkUser]);

  const logout = useCallback(async () => {
    await signOut();
    setUser(null);
  }, []);

  const registerBusiness = useCallback(async (data: RegisterBusinessRequest) => {
    await authApi.registerBusiness(data);
    await fetchProfile();
  }, [fetchProfile]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated,
      isLoading,
      signInWithEmail,
      signUpWithEmail,
      confirmSignUpCode,
      signInWithGoogle,
      logout,
      registerBusiness,
    }),
    [
      user,
      isAuthenticated,
      isLoading,
      signInWithEmail,
      signUpWithEmail,
      confirmSignUpCode,
      signInWithGoogle,
      logout,
      registerBusiness,
    ]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
