import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { describe, expect, it, vi } from 'vitest';
import LoginPage from '../features/auth/pages/LoginPage';

const loginMock = vi.fn();

vi.mock('../app/hooks/useAuth', () => ({
  useAuth: () => ({
    user: null,
    token: null,
    isAdmin: false,
    isAuthenticated: false,
    login: loginMock,
    logout: vi.fn(),
  }),
}));

function renderLogin() {
  return render(
    <MemoryRouter>
      <LoginPage />
    </MemoryRouter>,
  );
}

describe('LoginPage validation', () => {
  it('shows errors when submitting empty form', async () => {
    const user = userEvent.setup();
    renderLogin();

    await user.click(screen.getByRole('button', { name: /iniciar sesión/i }));

    expect(screen.getByText('El correo es obligatorio.')).toBeInTheDocument();
    expect(screen.getByText('La contraseña es obligatoria.')).toBeInTheDocument();
    expect(loginMock).not.toHaveBeenCalled();
  });

  it('shows email format error for invalid email', async () => {
    const user = userEvent.setup();
    renderLogin();

    await user.type(screen.getByLabelText(/correo/i), 'not-an-email');
    await user.type(screen.getByLabelText(/contraseña/i), 'Admin@123');
    await user.click(screen.getByRole('button', { name: /iniciar sesión/i }));

    expect(screen.getByText('Introduce un correo electrónico válido.')).toBeInTheDocument();
    expect(loginMock).not.toHaveBeenCalled();
  });

  it('submits with valid credentials', async () => {
    const user = userEvent.setup();
    loginMock.mockResolvedValue({ email: 'admin@consultora.test', role: 'Admin' });
    renderLogin();

    await user.type(screen.getByLabelText(/correo/i), 'admin@consultora.test');
    await user.type(screen.getByLabelText(/contraseña/i), 'Admin@123');
    await user.click(screen.getByRole('button', { name: /iniciar sesión/i }));

    expect(loginMock).toHaveBeenCalledWith('admin@consultora.test', 'Admin@123');
  });
});