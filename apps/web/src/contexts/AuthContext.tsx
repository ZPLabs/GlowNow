"use client";

import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import type { User, AuthState, LoginRequest, RegisterRequest } from "@/types/auth";
import * as authApi from "@/lib/api/auth";
import {
  getStoredRefreshToken,
  setStoredRefreshToken,
  clearStoredRefreshToken,
  isTokenExpired,
} from "@/lib/auth/tokens";

export interface AuthContextValue extends AuthState {
  login: (data: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  refreshSession: () => Promise<boolean>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [tokenIssuedAt, setTokenIssuedAt] = useState<number | null>(null);
  const [expiresIn, setExpiresIn] = useState<number | null>(null);

  const isAuthenticated = !!user && !!accessToken;

  const refreshSession = useCallback(async (): Promise<boolean> => {
    const storedRefreshToken = getStoredRefreshToken();
    if (!storedRefreshToken) {
      return false;
    }

    try {
      const response = await authApi.refreshToken({
        refreshToken: storedRefreshToken,
      });

      setAccessToken(response.accessToken);
      setStoredRefreshToken(response.refreshToken);
      setTokenIssuedAt(Date.now());
      setExpiresIn(response.expiresIn);

      const currentUser = await authApi.getCurrentUser(response.accessToken);
      setUser(currentUser);

      return true;
    } catch {
      clearStoredRefreshToken();
      setUser(null);
      setAccessToken(null);
      setTokenIssuedAt(null);
      setExpiresIn(null);
      return false;
    }
  }, []);

  const login = useCallback(async (data: LoginRequest): Promise<void> => {
    const response = await authApi.login(data);

    setAccessToken(response.accessToken);
    setStoredRefreshToken(response.refreshToken);
    setTokenIssuedAt(Date.now());
    setExpiresIn(response.expiresIn);

    const currentUser = await authApi.getCurrentUser(response.accessToken);
    setUser(currentUser);
  }, []);

  const register = useCallback(async (data: RegisterRequest): Promise<void> => {
    await authApi.register(data);
  }, []);

  const logout = useCallback(async (): Promise<void> => {
    if (accessToken) {
      try {
        await authApi.logout(accessToken);
      } catch {
        // Ignore logout errors - we'll clear local state anyway
      }
    }

    clearStoredRefreshToken();
    setUser(null);
    setAccessToken(null);
    setTokenIssuedAt(null);
    setExpiresIn(null);
  }, [accessToken]);

  // Initialize auth state on mount
  useEffect(() => {
    const initAuth = async () => {
      const success = await refreshSession();
      if (!success) {
        setIsLoading(false);
      } else {
        setIsLoading(false);
      }
    };

    initAuth();
  }, [refreshSession]);

  // Auto-refresh token before expiration
  useEffect(() => {
    if (!tokenIssuedAt || !expiresIn || !accessToken) {
      return;
    }

    const checkAndRefresh = async () => {
      if (isTokenExpired(expiresIn, tokenIssuedAt)) {
        await refreshSession();
      }
    };

    // Check every minute
    const intervalId = setInterval(checkAndRefresh, 60 * 1000);

    return () => clearInterval(intervalId);
  }, [tokenIssuedAt, expiresIn, accessToken, refreshSession]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      accessToken,
      isAuthenticated,
      isLoading,
      login,
      register,
      logout,
      refreshSession,
    }),
    [
      user,
      accessToken,
      isAuthenticated,
      isLoading,
      login,
      register,
      logout,
      refreshSession,
    ]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
