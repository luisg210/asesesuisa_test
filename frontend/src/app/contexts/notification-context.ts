import { createContext } from 'react';

export type ToastSeverity = 'success' | 'info' | 'warning' | 'error';

export interface NotificationContextValue {
  showToast: (message: string, severity?: ToastSeverity) => void;
}

export const NotificationContext = createContext<NotificationContextValue | undefined>(undefined);