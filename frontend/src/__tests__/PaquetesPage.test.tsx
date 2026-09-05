import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import PaquetesPage from '../features/paquetes/pages/PaquetesPage';
import { createPaquete, deletePaquete, listAreas, listPaquetes } from '../shared/api/endpoints';
import { ToastProvider } from '../app/contexts/NotificationContext';
import type { Paquete } from '../shared/types';

vi.mock('../shared/api/endpoints', () => ({
  listPaquetes: vi.fn(),
  listAreas: vi.fn(),
  createPaquete: vi.fn(),
  updatePaquete: vi.fn(),
  deletePaquete: vi.fn(),
}));

const AREAS = ['Estrategia', 'Tecnologia', 'Finanzas', 'Recursos Humanos', 'Comercial'];

const paquetes: Paquete[] = [
  {
    id: 1,
    nombre: 'Diagnostico Estrategico',
    descripcion: 'Evaluacion inicial',
    area: 'Estrategia',
    precio: 3500,
    activo: true,
    fechaCreacion: '2026-01-01T00:00:00Z',
  },
  {
    id: 2,
    nombre: 'Plan de Transformacion Digital',
    area: 'Tecnologia',
    precio: 8500,
    activo: true,
    fechaCreacion: '2026-01-02T00:00:00Z',
  },
];

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
    user: { email: 'admin@consultora.test', role: isAdmin ? 'Admin' : 'User' },
    token: 'token',
    isAdmin,
    isAuthenticated: true,
    logout: vi.fn(),
  });
}

function renderPage() {
  return render(
    <MemoryRouter>
      <ToastProvider>
        <PaquetesPage />
      </ToastProvider>
    </MemoryRouter>,
  );
}

describe('PaquetesPage', () => {
  beforeEach(() => {
    vi.mocked(listPaquetes).mockResolvedValue({
      items: paquetes,
      totalCount: 2,
      page: 1,
      pageSize: 10,
      totalPages: 1,
    });
    vi.mocked(listAreas).mockResolvedValue(AREAS);
  });

  it('loads the area catalog from the backend into the filter', async () => {
    mockAuth(true);
    vi.mocked(listAreas).mockResolvedValue(['Finanzas', 'Tecnologia']);
    renderPage();

    await screen.findByText('Diagnostico Estrategico');
    await waitFor(() => {
      const select = screen.getByLabelText('Área') as HTMLSelectElement;
      expect([...select.options].map((option) => option.value)).toEqual(
        expect.arrayContaining(['Finanzas', 'Tecnologia']),
      );
    });
  });

  it('renders paquetes from the API', async () => {
    mockAuth(true);
    renderPage();

    expect(await screen.findByText('Diagnostico Estrategico')).toBeInTheDocument();
    expect(await screen.findByText('Plan de Transformacion Digital')).toBeInTheDocument();
    expect(screen.getByText(/3500,00/)).toBeInTheDocument();
    expect(listPaquetes).toHaveBeenCalled();
  });

  it('renders empty state when there are no paquetes', async () => {
    mockAuth(true);
    vi.mocked(listPaquetes).mockResolvedValue({
      items: [],
      totalCount: 0,
      page: 1,
      pageSize: 10,
      totalPages: 0,
    });
    renderPage();

    expect(await screen.findByText('No hay paquetes.')).toBeInTheDocument();
  });

  it('filters by search text and calls the API again', async () => {
    mockAuth(true);
    renderPage();
    await screen.findByText('Diagnostico Estrategico');

    await userEvent.type(screen.getByLabelText(/buscar por nombre/i), 'digital');

    await waitFor(() => {
      expect(listPaquetes).toHaveBeenCalledWith(
        expect.objectContaining({ nombre: 'digital' }),
      );
    });
  });

  it('shows a success toast after creating a paquete', async () => {
    mockAuth(true);
    vi.mocked(createPaquete).mockResolvedValue(paquetes[0]);
    renderPage();
    await screen.findByText('Diagnostico Estrategico');

    await userEvent.click(screen.getByRole('button', { name: /nuevo paquete/i }));
    const dialog = screen.getByRole('dialog');
    await userEvent.type(within(dialog).getByLabelText('Nombre'), 'Paquete Nuevo');
    await userEvent.selectOptions(within(dialog).getByLabelText('Área'), 'Finanzas');
    await userEvent.click(within(dialog).getByRole('button', { name: /guardar/i }));

    expect(await screen.findByText('Paquete creado correctamente.')).toBeInTheDocument();
    expect(createPaquete).toHaveBeenCalledWith(expect.objectContaining({ nombre: 'Paquete Nuevo' }));
  });

  it('shows an error toast when creating a paquete fails', async () => {
    mockAuth(true);
    vi.mocked(createPaquete).mockRejectedValue({
      isAxiosError: true,
      response: { data: { message: 'Ya existe un paquete con ese nombre.', data: null } },
    });
    renderPage();
    await screen.findByText('Diagnostico Estrategico');

    await userEvent.click(screen.getByRole('button', { name: /nuevo paquete/i }));
    const dialog = screen.getByRole('dialog');
    await userEvent.type(within(dialog).getByLabelText('Nombre'), 'Duplicado');
    await userEvent.selectOptions(within(dialog).getByLabelText('Área'), 'Finanzas');
    await userEvent.click(within(dialog).getByRole('button', { name: /guardar/i }));

    expect(
      await screen.findByText('Ya existe un paquete con ese nombre.'),
    ).toBeInTheDocument();
  });

  it('shows a success toast after deleting a paquete', async () => {
    mockAuth(true);
    vi.mocked(deletePaquete).mockResolvedValue(undefined);
    renderPage();
    await screen.findByText('Diagnostico Estrategico');

    await userEvent.click(screen.getByLabelText('Eliminar paquete Diagnostico Estrategico'));
    await userEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: /eliminar/i }));

    expect(await screen.findByText('Paquete eliminado correctamente.')).toBeInTheDocument();
    expect(deletePaquete).toHaveBeenCalledWith(1);
  });

  it('shows an error toast when deleting a paquete fails', async () => {
    mockAuth(true);
    vi.mocked(deletePaquete).mockRejectedValue({
      isAxiosError: true,
      response: { data: { message: 'No se pudo eliminar el paquete.', data: null } },
    });
    renderPage();
    await screen.findByText('Diagnostico Estrategico');

    await userEvent.click(screen.getByLabelText('Eliminar paquete Diagnostico Estrategico'));
    await userEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: /eliminar/i }));

    expect(
      await screen.findByText('No se pudo eliminar el paquete.'),
    ).toBeInTheDocument();
  });
});