import { ApiException } from "@/types/api";
import type { ApiError, ApiResponse } from "@/types/api";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5249";

type RequestOptions = {
  method?: "GET" | "POST" | "PUT" | "DELETE" | "PATCH";
  body?: unknown;
  headers?: Record<string, string>;
  accessToken?: string | null;
};

export async function apiClient<T>(
  endpoint: string,
  options: RequestOptions = {}
): Promise<T> {
  const { method = "GET", body, headers = {}, accessToken } = options;

  const requestHeaders: Record<string, string> = {
    "Content-Type": "application/json",
    ...headers,
  };

  if (accessToken) {
    requestHeaders["Authorization"] = `Bearer ${accessToken}`;
  }

  const response = await fetch(`${API_URL}${endpoint}`, {
    method,
    headers: requestHeaders,
    body: body ? JSON.stringify(body) : undefined,
    credentials: "include",
  });

  if (!response.ok) {
    const errorData: ApiError = await response.json().catch(() => ({
      error: {
        code: "UNKNOWN_ERROR",
        message: "An unexpected error occurred",
      },
    }));

    throw new ApiException(
      errorData.error.message,
      errorData.error.code,
      response.status,
      errorData.error.details
    );
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const data: ApiResponse<T> = await response.json();
  return data.data;
}
