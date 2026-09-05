import { useCallback, useMemo, useState } from 'react';
import { listAuditoria } from '../../../shared/api/endpoints';
import type { ListParams } from '../../../shared/api/params';
import { usePagedList } from '../../../shared/hooks/usePagedList';
import type { AuditoriaEntry } from '../../../shared/types';

export interface AuditoriaFilters {
  entidad: string;
  accion: string;
  usuario: string;
}

/** Listado paginado y filtros de la bitacora de auditoria. */
export function useAuditoria() {
  const [filters, setFilters] = useState<AuditoriaFilters>({ entidad: '', accion: '', usuario: '' });

  const params = useMemo<ListParams>(
    () => ({
      entidad: filters.entidad.trim() || undefined,
      accion: filters.accion.trim() || undefined,
      usuario: filters.usuario.trim() || undefined,
    }),
    [filters],
  );

  const list = usePagedList<AuditoriaEntry, ListParams>({
    params,
    fetcher: listAuditoria,
    defaultSortBy: 'FechaHora',
    defaultSortDir: 'desc',
    errorMessage: 'No se pudo cargar la bitácora. Revisa la conexión con la API.',
  });

  const { setPage } = list;

  const setFilter = useCallback(
    (patch: Partial<AuditoriaFilters>) => {
      setFilters((current) => ({ ...current, ...patch }));
      setPage(0);
    },
    [setPage],
  );

  return {
    ...list,
    filters,
    setFilter,
  };
}