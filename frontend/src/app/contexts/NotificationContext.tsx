import { useCallback, useState, type ReactNode } from "react";
import { Alert, Snackbar } from "@mui/material";
import { NotificationContext, type ToastSeverity } from "./notification-context";

interface ToastState {
  open: boolean;
  message: string;
  severity: ToastSeverity;
}

const initialToast: ToastState = { open: false, message: "", severity: "success" };

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toast, setToast] = useState<ToastState>(initialToast);

  const showToast = useCallback((message: string, severity: ToastSeverity = "success") => {
    setToast({ open: true, message, severity });
  }, []);

  const handleClose = useCallback(
    (_event: React.SyntheticEvent | Event, reason?: string) => {
      if (reason === "clickaway") return;
      setToast((current) => ({ ...current, open: false }));
    },
    [],
  );

  return (
    <NotificationContext.Provider value={{ showToast }}>
      {children}
      <Snackbar
        open={toast.open}
        autoHideDuration={4000}
        onClose={handleClose}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert onClose={handleClose} severity={toast.severity} variant="filled">
          {toast.message}
        </Alert>
      </Snackbar>
    </NotificationContext.Provider>
  );
}