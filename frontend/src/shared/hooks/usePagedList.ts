import { useCallback, useEffect, useMemo, useState } from 'react';
import type { PagedResult } from '../types';

export interface PagedListState {
  page: number;
  pageSize: number;
  sortBy: string;
  sortDir: 'asc' | 'desc';
}

export interface UsePagedListResult<T> {
  rows: T[];
  totalCount: number;
  loading: boolean;
  error: string;
  page: number;
  pageSize: number;
  sortBy: string;
  sortDir: 'asc' | 'desc';
  setPage: (page: number) => void;
  setPageSize: (pageSize: number) => void;
  handleSort: (column: string) => void;
  reload: () => void;
}

interface UsePagedListOptions<T, P> {
  params: P;
  fetcher: (params: P) => Promise<PagedResult<T>>;
  defaultSortBy?: string;
  defaultSortDir?: 'asc' | 'desc';
  errorMessage?: string;
}

/**
 * Maneja el estado comun de una lista paginada: pagina, tamano de pagina,
 * orden y la peticion a la API. Los filtros se pasan como `params`; cualquier
 * cambio en su referencia dispara una nueva carga.
 */
export function usePagedList<T, P extends object>(options: UsePagedListOptions<T, P>): UsePagedListResult<T> {
  const {
    params,
    fetcher,
    defaultSortBy = 'Id',
    defaultSortDir = 'asc',
    errorMessage = 'No se pudieron cargar los datos. Revisa la conexión con la API.',
  } = options;

  const [state, setState] = useState<PagedListState>({
    page: 0,
    pageSize: 10,
    sortBy: defaultSortBy,
    sortDir: defaultSortDir,
  });
  const [rows, setRows] = useState<T[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [reloadToken, setReloadToken] = useState(0);

  const requestParams = useMemo(
    () =>
      ({
        ...(params as object),
        page: state.page + 1,
        pageSize: state.pageSize,
        sortBy: state.sortBy,
        sortDir: state.sortDir,
      }) as P,
    [params, state.page, state.pageSize, state.sortBy, state.sortDir],
  );

  useEffect(() => {
    let cancelled = false;
    const load = async () => {
      setLoading(true);
      setError('');
      try {
        const result = await fetcher(requestParams);
        if (cancelled) return;
        setRows(result.items);
        setTotalCount(result.totalCount);
      } catch {
        if (cancelled) return;
        setError(errorMessage);
      } finally {
        if (!cancelled) setLoading(false);
      }
    };
    void load();
    return () => {
      cancelled = true;
    };
  }, [fetcher, requestParams, reloadToken, errorMessage]);

  const setPage = useCallback((page: number) => {
    setState((current) => ({ ...current, page }));
  }, []);

  const setPageSize = useCallback((pageSize: number) => {
    setState((current) => ({ ...current, pageSize, page: 0 }));
  }, []);

  const handleSort = useCallback((column: string) => {
    setState((current) =>
      current.sortBy === column
        ? { ...current, sortDir: current.sortDir === 'asc' ? 'desc' : 'asc' }
        : { ...current, sortBy: column, sortDir: 'asc' },
    );
  }, []);

  const reload = useCallback(() => setReloadToken((token) => token + 1), []);

  return {
    rows,
    totalCount,
    loading,
    error,
    page: state.page,
    pageSize: state.pageSize,
    sortBy: state.sortBy,
    sortDir: state.sortDir,
    setPage,
    setPageSize,
    handleSort,
    reload,
  };
}