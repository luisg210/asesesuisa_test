import { useContext } from 'react';
import { NotificationContext, type NotificationContextValue } from '../contexts/notification-context';

export function useToast(): NotificationContextValue {
  const ctx = useContext(NotificationContext);
  if (!ctx) {
    throw new Error('useToast must be used within a ToastProvider.');
  }
  return ctx;
}