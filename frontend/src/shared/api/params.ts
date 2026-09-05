export interface ListParams {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDir?: 'asc' | 'desc';
  nombre?: string;
  area?: string;
  activo?: boolean;
  entidad?: string;
  accion?: string;
  usuario?: string;
}