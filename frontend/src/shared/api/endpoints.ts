import { api } from './client';
import type { ListParams } from './params';
import type {
  ApiResponse,
  AuditoriaEntry,
  Consultor,
  ConsultorPaquete,
  ConsultorPayload,
  PagedResult,
  Paquete,
  PaquetePayload,
  PaquetePorArea,
  ConsultorFacturacion,
} from '../types';

function cleanParams(params: object) {
  const clean: Record<string, string | number | boolean> = {};
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') clean[key] = value as string | number | boolean;
  });
  return clean;
}

export async function listAreas(): Promise<string[]> {
  const { data } = await api.get<ApiResponse<string[]>>('/areas');
  return data.data;
}

export async function listPaquetes(params: ListParams): Promise<PagedResult<Paquete>> {
  const { data } = await api.get<ApiResponse<PagedResult<Paquete>>>('/paquetes', {
    params: cleanParams(params),
  });
  return data.data;
}

export async function getPaquete(id: number): Promise<Paquete> {
  const { data } = await api.get<ApiResponse<Paquete>>(`/paquetes/${id}`);
  return data.data;
}

export async function createPaquete(payload: Omit<PaquetePayload, 'activo'>): Promise<Paquete> {
  const { data } = await api.post<ApiResponse<Paquete>>('/paquetes', payload);
  return data.data;
}

export async function updatePaquete(id: number, payload: PaquetePayload): Promise<Paquete> {
  const { data } = await api.put<ApiResponse<Paquete>>(`/paquetes/${id}`, payload);
  return data.data;
}

export async function deletePaquete(id: number): Promise<void> {
  await api.delete(`/paquetes/${id}`);
}

export async function listConsultores(params: ListParams): Promise<PagedResult<Consultor>> {
  const { data } = await api.get<ApiResponse<PagedResult<Consultor>>>('/consultores', {
    params: cleanParams(params),
  });
  return data.data;
}

export async function createConsultor(payload: Omit<ConsultorPayload, 'activo'>): Promise<Consultor> {
  const { data } = await api.post<ApiResponse<Consultor>>('/consultores', payload);
  return data.data;
}

export async function updateConsultor(id: number, payload: ConsultorPayload): Promise<Consultor> {
  const { data } = await api.put<ApiResponse<Consultor>>(`/consultores/${id}`, payload);
  return data.data;
}

export async function deleteConsultor(id: number): Promise<void> {
  await api.delete(`/consultores/${id}`);
}

export async function reportePaquetesPorArea(params: ListParams): Promise<PagedResult<PaquetePorArea>> {
  const { data } = await api.get<ApiResponse<PagedResult<PaquetePorArea>>>(
    '/reportes/paquetes-por-area',
    { params: cleanParams(params) },
  );
  return data.data;
}

export async function reporteTopFacturacion(params: ListParams): Promise<PagedResult<ConsultorFacturacion>> {
  const { data } = await api.get<ApiResponse<PagedResult<ConsultorFacturacion>>>(
    '/reportes/consultores-top-facturacion',
    { params: cleanParams(params) },
  );
  return data.data;
}

export async function listPaquetesByConsultor(consultorId: number): Promise<ConsultorPaquete[]> {
  const { data } = await api.get<ApiResponse<ConsultorPaquete[]>>(`/consultores/${consultorId}/paquetes`);
  return data.data;
}

export async function assignPaqueteToConsultor(consultorId: number, paqueteId: number): Promise<ConsultorPaquete[]> {
  const { data } = await api.post<ApiResponse<ConsultorPaquete[]>>(
    `/consultores/${consultorId}/paquetes`,
    { paqueteId },
  );
  return data.data;
}

export async function unassignPaqueteFromConsultor(consultorId: number, paqueteId: number): Promise<ConsultorPaquete[]> {
  const { data } = await api.delete<ApiResponse<ConsultorPaquete[]>>(`/consultores/${consultorId}/paquetes/${paqueteId}`);
  return data.data;
}

export async function listAuditoria(params: ListParams): Promise<PagedResult<AuditoriaEntry>> {
  const { data } = await api.get<ApiResponse<PagedResult<AuditoriaEntry>>>('/auditoria', {
    params: cleanParams(params),
  });
  return data.data;
}