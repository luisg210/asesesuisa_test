import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import PaquetesPage from '../features/paquetes/pages/PaquetesPage';
import { listAreas, listPaquetes } from '../shared/api/endpoints';
import { ToastProvider } from '../app/contexts/NotificationContext';

vi.mock('../shared/api/endpoints', () => ({
  listPaquetes: vi.fn(),
  listAreas: vi.fn(),
  createPaquete: vi.fn(),
  updatePaquete: vi.fn(),
  deletePaquete: vi.fn(),
}));

const authMock = vi.fn();

vi.mock('../app/hooks/useAuth', () => ({
  useAuth: () => authMock() as {
    user: unknown;
    token: unknown;
    isAdmin: boolean;
    isAuthenticated: boolean;
    logout: () => void;
  },
}));

function mockAuth(isAdmin: boolean) {
  authMock.mockReturnValue({
    user: { email: 'x@y.test', role: isAdmin ? 'Admin' : 'User' },
    token: 'token',
    isAdmin,
    isAuthenticated: true,
    logout: vi.fn(),
  });
}

describe('Role visibility', () => {
  beforeEach(() => {
    vi.mocked(listPaquetes).mockResolvedValue({
      items: [
        {
          id: 1,
          nombre: 'Paquete A',
          area: 'Finanzas',
          precio: 100,
          activo: true,
          fechaCreacion: '2026-01-01T00:00:00Z',
        },
      ],
      totalCount: 1,
      page: 1,
      pageSize: 10,
      totalPages: 1,
    });
    vi.mocked(listAreas).mockResolvedValue(['Finanzas']);
  });

  it('hides write actions for a User role', async () => {
    mockAuth(false);
    render(
      <MemoryRouter>
        <ToastProvider>
          <PaquetesPage />
        </ToastProvider>
      </MemoryRouter>,
    );

    await screen.findByText('Paquete A');
    expect(screen.queryByRole('button', { name: /nuevo paquete/i })).not.toBeInTheDocument();
    expect(screen.queryByLabelText(/editar paquete/i)).not.toBeInTheDocument();
  });

  it('shows write actions for an Admin role', async () => {
    mockAuth(true);
    render(
      <MemoryRouter>
        <ToastProvider>
          <PaquetesPage />
        </ToastProvider>
      </MemoryRouter>,
    );

    await screen.findByText('Paquete A');
    expect(screen.getByRole('button', { name: /nuevo paquete/i })).toBeInTheDocument();
  });
});