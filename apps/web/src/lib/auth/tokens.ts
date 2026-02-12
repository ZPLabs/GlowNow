const REFRESH_TOKEN_KEY = "glownow_refresh_token";

export function getStoredRefreshToken(): string | null {
  if (typeof window === "undefined") {
    return null;
  }
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function setStoredRefreshToken(token: string): void {
  if (typeof window === "undefined") {
    return;
  }
  localStorage.setItem(REFRESH_TOKEN_KEY, token);
}

export function clearStoredRefreshToken(): void {
  if (typeof window === "undefined") {
    return;
  }
  localStorage.removeItem(REFRESH_TOKEN_KEY);
}

export function isTokenExpired(expiresIn: number, issuedAt: number): boolean {
  const expirationTime = issuedAt + expiresIn * 1000;
  const bufferTime = 60 * 1000;
  return Date.now() >= expirationTime - bufferTime;
}
