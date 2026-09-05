import { useCallback, useMemo, useState } from 'react';
import {
  createPaquete,
  deletePaquete,
  listPaquetes,
  updatePaquete,
} from '../../../shared/api/endpoints';
import type { ListParams } from '../../../shared/api/params';
import { usePagedList } from '../../../shared/hooks/usePagedList';
import type { Paquete, PaquetePayload } from '../../../shared/types';

export interface PaqueteFilters {
  nombre: string;
  area: string;
  activo: string;
}

/** Estado y operaciones CRUD de la pagina de paquetes. */
export function usePaquetes() {
  const [filters, setFilters] = useState<PaqueteFilters>({ nombre: '', area: '', activo: '' });

  const params = useMemo<ListParams>(
    () => ({
      nombre: filters.nombre.trim() || undefined,
      area: filters.area || undefined,
      activo: filters.activo === '' ? undefined : filters.activo === 'active',
    }),
    [filters],
  );

  const list = usePagedList<Paquete, ListParams>({
    params,
    fetcher: listPaquetes,
    defaultSortBy: 'Id',
    defaultSortDir: 'asc',
    errorMessage: 'No se pudieron cargar los paquetes. Revisa la conexión con la API.',
  });

  const { setPage } = list;

  const setFilter = useCallback(
    (patch: Partial<PaqueteFilters>) => {
      setFilters((current) => ({ ...current, ...patch }));
      setPage(0);
    },
    [setPage],
  );

  const create = useCallback(
    (payload: Omit<PaquetePayload, 'activo'>) => createPaquete(payload),
    [],
  );
  const update = useCallback((id: number, payload: PaquetePayload) => updatePaquete(id, payload), []);
  const remove = useCallback((id: number) => deletePaquete(id), []);

  return {
    ...list,
    filters,
    setFilter,
    create,
    update,
    remove,
  };
}