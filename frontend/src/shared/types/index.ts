export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data: T;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface Paquete {
  id: number;
  nombre: string;
  descripcion?: string;
  area: string;
  precio: number;
  activo: boolean;
  fechaCreacion: string;
}

export interface PaquetePayload {
  nombre: string;
  descripcion?: string;
  area: string;
  precio: number;
  activo: boolean;
}

export interface Consultor {
  id: number;
  nombreCompleto: string;
  email: string;
  area: string;
  tarifaHora: number;
  activo: boolean;
  proyectosActivos: number;
  fechaCreacion: string;
}

export interface ConsultorPayload {
  nombreCompleto: string;
  email: string;
  area: string;
  tarifaHora: number;
  activo: boolean;
  proyectosActivos: number;
}

export interface PaquetePorArea {
  area: string;
  totalPaquetes: number;
  totalMonto: number;
  precioMinimo: number;
  precioMaximo: number;
}

export interface ConsultorFacturacion {
  id: number;
  nombreCompleto: string;
  email: string;
  area: string;
  tarifaHora: number;
  proyectosActivos: number;
  facturacionEstimada: number;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  refreshToken: string;
  refreshExpiresAt: string;
  email: string;
  role: string;
}

export interface AuthUser {
  email: string;
  role: string;
}

export interface ConsultorPaquete {
  paqueteId: number;
  nombre: string;
  descripcion?: string;
  area: string;
  precio: number;
  activo: boolean;
  fechaAsignacion: string;
}

export interface AuditoriaEntry {
  id: number;
  usuario: string;
  accion: string;
  entidad: string;
  entidadId?: number;
  detalle?: string;
  ip?: string;
  fechaHora: string;
}