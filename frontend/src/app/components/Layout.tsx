import {
  AppBar,
  Box,
  Button,
  Container,
  IconButton,
  Toolbar,
  Typography,
} from "@mui/material";
import { DarkMode, LightMode } from "@mui/icons-material";
import { Link as RouterLink, NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { useThemeMode } from "../hooks/useThemeMode";

const navItems = [
  { label: "Paquetes", to: "/paquetes" },
  { label: "Consultores", to: "/consultores" },
  { label: "Reportes", to: "/reportes" },
];

export default function Layout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  const { user, logout, isAdmin } = useAuth();
  const { mode, toggleMode } = useThemeMode();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <Box sx={{ minHeight: "100vh", display: "flex", flexDirection: "column" }}>
      <AppBar position="static">
        <Toolbar>
          <Typography
            variant="h6"
            component={RouterLink}
            to="/paquetes"
            sx={{ textDecoration: "none", color: "inherit", mr: 4 }}
          >
            Consultora
          </Typography>
          <Box sx={{ display: "flex", gap: 2, flexGrow: 1 }}>
            {navItems.map((item) => (
              <Button
                key={item.to}
                component={NavLink}
                to={item.to}
                color="inherit"
                sx={{
                  "&.active": {
                    fontWeight: "bold",
                    borderBottom: "2px solid white",
                  },
                }}
              >
                {item.label}
              </Button>
            ))}
            {isAdmin && (
              <Button
                component={NavLink}
                to="/auditoria"
                color="inherit"
                sx={{
                  "&.active": {
                    fontWeight: "bold",
                    borderBottom: "2px solid white",
                  },
                }}
              >
                Auditoría
              </Button>
            )}
          </Box>
          <Typography variant="body2" sx={{ mr: 2 }}>
            {user?.email} (
            {user?.role === "Admin" ? "Administrador" : "Usuario"})
          </Typography>
          <IconButton
            color="inherit"
            onClick={toggleMode}
            aria-label={
              mode === "light" ? "Activar tema oscuro" : "Activar tema claro"
            }
          >
            {mode === "light" ? <DarkMode /> : <LightMode />}
          </IconButton>
          <Button color="inherit" onClick={handleLogout}>
            Salir
          </Button>
        </Toolbar>
      </AppBar>
      <Container maxWidth="lg" sx={{ py: 4, flexGrow: 1 }}>
        {children}
      </Container>
    </Box>
  );
}
