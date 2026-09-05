import axios from 'axios';

interface ApiErrorBody {
  message?: string;
  data?: Array<{ propertyName?: string; errorMessage?: string }> | null;
}

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError<ApiErrorBody>(error)) {
    const firstValidationError = error.response?.data?.data?.[0]?.errorMessage;
    if (firstValidationError) return firstValidationError;
    if (error.response?.data?.message) return error.response.data.message;
    if (error.code === 'ERR_NETWORK') {
      return 'No se pudo conectar con el servidor. Inténtalo de nuevo.';
    }
  }
  return 'Ocurrió un error inesperado.';
}