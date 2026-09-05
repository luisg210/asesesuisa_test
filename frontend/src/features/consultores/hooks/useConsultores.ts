import { useCallback, useMemo, useState } from 'react';
import {
  createConsultor,
  deleteConsultor,
  listConsultores,
  updateConsultor,
} from '../../../shared/api/endpoints';
import type { ListParams } from '../../../shared/api/params';
import { usePagedList } from '../../../shared/hooks/usePagedList';
import type { Consultor, ConsultorPayload } from '../../../shared/types';

export interface ConsultorFilters {
  nombre: string;
  area: string;
  activo: string;
}

/** Estado y operaciones CRUD de la pagina de consultores. */
export function useConsultores() {
  const [filters, setFilters] = useState<ConsultorFilters>({ nombre: '', area: '', activo: '' });

  const params = useMemo<ListParams>(
    () => ({
      nombre: filters.nombre.trim() || undefined,
      area: filters.area || undefined,
      activo: filters.activo === '' ? undefined : filters.activo === 'active',
    }),
    [filters],
  );

  const list = usePagedList<Consultor, ListParams>({
    params,
    fetcher: listConsultores,
    defaultSortBy: 'Id',
    defaultSortDir: 'asc',
    errorMessage: 'No se pudieron cargar los consultores. Revisa la conexión con la API.',
  });

  const { setPage } = list;

  const setFilter = useCallback(
    (patch: Partial<ConsultorFilters>) => {
      setFilters((current) => ({ ...current, ...patch }));
      setPage(0);
    },
    [setPage],
  );

  const create = useCallback(
    (payload: Omit<ConsultorPayload, 'activo'>) => createConsultor(payload),
    [],
  );
  const update = useCallback(
    (id: number, payload: ConsultorPayload) => updateConsultor(id, payload),
    [],
  );
  const remove = useCallback((id: number) => deleteConsultor(id), []);

  return {
    ...list,
    filters,
    setFilter,
    create,
    update,
    remove,
  };
}