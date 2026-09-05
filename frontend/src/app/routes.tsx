import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import PrivateRoute from './components/PrivateRoute';
import Layout from './components/Layout';
import LoginPage from '../features/auth/pages/LoginPage';
import PaquetesPage from '../features/paquetes/pages/PaquetesPage';
import ConsultoresPage from '../features/consultores/pages/ConsultoresPage';
import ReportesPage from '../features/reportes/pages/ReportesPage';
import AuditoriaPage from '../features/auditoria/pages/AuditoriaPage';

export default function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route
          path="/"
          element={
            <PrivateRoute>
              <Layout>
                <PaquetesPage />
              </Layout>
            </PrivateRoute>
          }
        />
        <Route
          path="/paquetes"
          element={
            <PrivateRoute>
              <Layout>
                <PaquetesPage />
              </Layout>
            </PrivateRoute>
          }
        />
        <Route
          path="/consultores"
          element={
            <PrivateRoute>
              <Layout>
                <ConsultoresPage />
              </Layout>
            </PrivateRoute>
          }
        />
        <Route
          path="/reportes"
          element={
            <PrivateRoute>
              <Layout>
                <ReportesPage />
              </Layout>
            </PrivateRoute>
          }
        />
        <Route
          path="/auditoria"
          element={
            <PrivateRoute adminOnly>
              <Layout>
                <AuditoriaPage />
              </Layout>
            </PrivateRoute>
          }
        />
        <Route path="*" element={<Navigate to="/paquetes" replace />} />
      </Routes>
    </BrowserRouter>
  );
}