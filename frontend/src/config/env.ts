export const API_URL =
  (import.meta.env.VITE_API_URL as string | undefined) ??
  'http://localhost:5058/api/v1';

export const TOKEN_KEY = 'consultora_token';
export const REFRESH_TOKEN_KEY = 'consultora_refresh_token';
export const USER_KEY = 'consultora_user';