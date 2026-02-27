import { fetchAuthSession } from 'aws-amplify/auth';

const API_URL = process.env.EXPO_PUBLIC_API_URL ?? "http://localhost:5249";

// On Android emulator, localhost is 10.0.2.2
const getApiUrl = () => {
  if (__DEV__ && API_URL.includes('localhost')) {
    return API_URL.replace('localhost', '10.0.2.2');
  }
  return API_URL;
};

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
    const accessToken = session.tokens?.accessToken?.toString();
    if (accessToken) {
      requestHeaders["Authorization"] = `Bearer ${accessToken}`;
    }
  } catch (error) {
    console.debug('Failed to fetch auth session', error);
  }

  const response = await fetch(`${getApiUrl()}${endpoint}`, {
    method,
    headers: requestHeaders,
    body: body ? JSON.stringify(body) : undefined,
  });

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.error?.message || "An unexpected error occurred");
  }

  return data.data;
}
