import { apiClient } from "./client";
import type {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  RefreshTokenRequest,
  RefreshTokenResponse,
  User,
} from "./types";

export async function login(data: LoginRequest): Promise<LoginResponse> {
  return apiClient<LoginResponse>("/api/v1/auth/login", {
    method: "POST",
    body: data,
  });
}

export async function register(data: RegisterRequest): Promise<RegisterResponse> {
  return apiClient<RegisterResponse>("/api/v1/auth/register", {
    method: "POST",
    body: data,
  });
}

export async function refreshToken(
  data: RefreshTokenRequest
): Promise<RefreshTokenResponse> {
  return apiClient<RefreshTokenResponse>("/api/v1/auth/refresh", {
    method: "POST",
    body: data,
  });
}

export async function logout(accessToken: string): Promise<void> {
  return apiClient<void>("/api/v1/auth/logout", {
    method: "POST",
    accessToken,
  });
}

export async function getCurrentUser(accessToken: string): Promise<User> {
  return apiClient<User>("/api/v1/auth/me", {
    method: "GET",
    accessToken,
  });
}
