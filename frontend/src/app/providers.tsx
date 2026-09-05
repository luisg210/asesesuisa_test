import type { ReactNode } from 'react';
import { AuthProvider } from './contexts/AuthContext';
import { ToastProvider } from './contexts/NotificationContext';
import { AppThemeProvider } from './contexts/ThemeContext';

export function AppProviders({ children }: { children: ReactNode }) {
  return (
    <AppThemeProvider>
      <AuthProvider>
        <ToastProvider>{children}</ToastProvider>
      </AuthProvider>
    </AppThemeProvider>
  );
}