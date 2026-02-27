import { fetchAuthSession } from 'aws-amplify/auth';
import { ApiException } from "@/types/api";
import type { ApiError, ApiResponse } from "@/types/api";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5249";

type RequestOptions = {
  method?: "GET" | "POST" | "PUT" | "DELETE" | "PATCH";
  body?: unknown;
  headers?: Record<string, string>;
};

export async function apiClient<T>(
  endpoint: string,
  options: RequestOptions = {}
): Promise<T> {
  const { method = "GET", body, headers = {} } = options;

  const requestHeaders: Record<string, string> = {
    "Content-Type": "application/json",
    ...headers,
  };

  try {
    const session = await fetchAuthSession();
    // Use the ID token: it contains user attributes (email, given_name, family_name)
    // needed by the API's lazy user creation middleware. The access token only has `sub`.
    const idToken = session.tokens?.idToken?.toString();
    if (idToken) {
      requestHeaders["Authorization"] = `Bearer ${idToken}`;
    }
  } catch (error) {
    // Session fetching failed - possibly not logged in
    console.debug('Failed to fetch auth session', error);
  }

  const response = await fetch(`${API_URL}${endpoint}`, {
    method,
    headers: requestHeaders,
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const body = await response.json().catch(() => null);

    // Handle our API's error shape: { error: { code, message, details } }
    // Fall back gracefully for ASP.NET ProblemDetails or empty bodies
    const message: string =
      body?.error?.message ?? body?.title ?? "An unexpected error occurred";
    const code: string =
      body?.error?.code ?? `HTTP_${response.status}`;
    const details = body?.error?.details;

    throw new ApiException(message, code, response.status, details);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const data: ApiResponse<T> = await response.json();
  return data.data;
}
