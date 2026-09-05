import { REFRESH_TOKEN_KEY, TOKEN_KEY, USER_KEY } from '../../config/env';
import type { AuthUser, LoginResponse } from '../types';

export interface Session {
  token: string | null;
  refreshToken: string | null;
  user: AuthUser | null;
}

export function persistSession(response: LoginResponse): void {
  localStorage.setItem(TOKEN_KEY, response.token);
  localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
  localStorage.setItem(
    USER_KEY,
    JSON.stringify({ email: response.email, role: response.role } satisfies AuthUser),
  );
}

export function readSession(): Session {
  const token = localStorage.getItem(TOKEN_KEY);
  const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
  const rawUser = localStorage.getItem(USER_KEY);
  if (!token || !refreshToken || !rawUser) return { token: null, refreshToken: null, user: null };

  try {
    return { token, refreshToken, user: JSON.parse(rawUser) as AuthUser };
  } catch {
    return { token: null, refreshToken: null, user: null };
  }
}

export function clearSession(): void {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}