import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { API_URL, TOKEN_KEY } from '../../config/env';
import { clearSession, persistSession, readSession } from '../auth/session';
import type { ApiResponse, LoginResponse } from '../types';

export const api = axios.create({
  baseURL: API_URL,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_KEY);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

interface RetriableRequest extends InternalAxiosRequestConfig {
  _retried?: boolean;
}

let refreshPromise: Promise<boolean> | null = null;

/** Renueva la sesion usando el refresh token (single-flight). */
function requestAccessToken(): Promise<boolean> {
  if (refreshPromise) return refreshPromise;

  const session = readSession();
  if (!session.refreshToken) return Promise.resolve(false);

  refreshPromise = axios
    .post<ApiResponse<LoginResponse>>(`${API_URL}/auth/refresh`, {
      refreshToken: session.refreshToken,
    })
    .then(({ data }) => {
      persistSession(data.data);
      return true;
    })
    .catch(() => {
      clearSession();
      return false;
    })
    .finally(() => {
      refreshPromise = null;
    });

  return refreshPromise;
}

function redirectToLogin(): void {
  if (!window.location.pathname.startsWith('/login')) {
    window.location.href = '/login';
  }
}

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const config = error.config as RetriableRequest | undefined;
    const url = config?.url ?? '';
    const isAuthEndpoint =
      url.includes('/auth/login') || url.includes('/auth/refresh') || url.includes('/auth/logout');

    // 401 no atribuible a login/refresh/logout: intentamos renovar el JWT una vez.
    if (error.response?.status === 401 && config && !config._retried && !isAuthEndpoint) {
      config._retried = true;
      const refreshed = await requestAccessToken();
      if (refreshed) {
        return api.request(config);
      }
      redirectToLogin();
    }

    return Promise.reject(error);
  },
);