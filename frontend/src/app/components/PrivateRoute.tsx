import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

interface PrivateRouteProps {
  children: React.ReactNode;
  /** Requiere rol Admin (p. ej. pagina de auditoria). */
  adminOnly?: boolean;
}

export default function PrivateRoute({
  children,
  adminOnly = false,
}: Readonly<PrivateRouteProps>) {
  const { isAuthenticated, isAdmin } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  if (adminOnly && !isAdmin) {
    return <Navigate to="/paquetes" replace />;
  }

  return <>{children}</>;
}
