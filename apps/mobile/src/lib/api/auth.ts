import { apiClient } from "./client";
import type {
  RegisterBusinessRequest,
  RegisterBusinessResponse,
  User,
} from "../../types/auth";

export async function getCurrentUser(): Promise<User> {
  return apiClient<User>("/api/v1/auth/me", {
    method: "GET",
  });
}

export async function registerBusiness(data: RegisterBusinessRequest): Promise<RegisterBusinessResponse> {
  return apiClient<RegisterBusinessResponse>("/api/v1/auth/register-business", {
    method: "POST",
    body: data,
  });
}
