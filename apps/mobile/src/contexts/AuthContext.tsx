import React, {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";
import { 
  signIn, 
  signUp, 
  confirmSignUp, 
  signOut, 
  getCurrentUser as getAmplifyUser,
  signInWithRedirect
} from 'aws-amplify/auth';
import { Hub } from 'aws-amplify/utils';
import type { User, AuthState, RegisterBusinessRequest } from "../types/auth";
import * as authApi from "../lib/api/auth";

export interface AuthContextValue extends AuthState {
  signInWithEmail: (email: string, password: string) => Promise<void>;
  signUpWithEmail: (email: string, password: string, firstName: string, lastName: string) => Promise<void>;
  confirmSignUpCode: (email: string, code: string) => Promise<void>;
  signInWithGoogle: () => Promise<void>;
  logout: () => Promise<void>;
  registerBusiness: (data: RegisterBusinessRequest) => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const isAuthenticated = !!user;

  const fetchProfile = useCallback(async () => {
    try {
      const profile = await authApi.getCurrentUser();
      setUser(profile);
    } catch (error) {
      console.debug('Failed to fetch user profile', error);
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

  const signUpWithEmail = useCallback(async (email: string, password: string, firstName: string, lastName: string) => {
    await signUp({
      username: email,
      password,
      options: {
        userAttributes: {
          email,
          given_name: firstName,
          family_name: lastName,
        }
      }
    });
  }, []);

  const confirmSignUpCode = useCallback(async (email: string, code: string) => {
    await confirmSignUp({ username: email, confirmationCode: code });
  }, []);

  const signInWithGoogle = useCallback(async () => {
    await signInWithRedirect({ provider: 'Google' });
  }, []);

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
