import { useMemo, useState, type ReactNode } from "react";
import {
  login as loginRequest,
  logout as logoutRequest,
} from "../../shared/auth/authApi";
import { persistSession, readSession } from "../../shared/auth/session";
import { AuthContext } from "./auth-context";

export function AuthProvider({ children }: Readonly<{ children: ReactNode }>) {
  const [session, setSession] = useState(readSession);

  const value = useMemo(
    () => ({
      user: session.user,
      token: session.token,
      isAdmin: session.user?.role === "Admin",
      isAuthenticated: Boolean(session.token && session.user),
      login: async (email: string, password: string) => {
        const response = await loginRequest(email, password);
        persistSession(response);
        setSession(readSession());
        return { email: response.email, role: response.role };
      },
      logout: () => {
        logoutRequest();
        setSession({ token: null, refreshToken: null, user: null });
      },
    }),
    [session],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}