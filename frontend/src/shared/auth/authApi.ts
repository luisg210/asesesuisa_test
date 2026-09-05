import axios from 'axios';
import { api } from '../api/client';
import { API_URL } from '../../config/env';
import { clearSession, readSession } from './session';
import type { ApiResponse, LoginResponse } from '../types';

export async function login(email: string, password: string): Promise<LoginResponse> {
  const { data } = await api.post<ApiResponse<LoginResponse>>('/auth/login', {
    email,
    password,
  });
  return data.data;
}

/** Limpia la sesion local y cierra la sesion remota (best-effort). */
export function logout(): void {
  const session = readSession();
  if (session.refreshToken) {
    void axios
      .post(`${API_URL}/auth/logout`, { refreshToken: session.refreshToken })
      .catch(() => undefined);
  }
  clearSession();
}