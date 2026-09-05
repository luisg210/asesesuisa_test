import { useCallback, useEffect, useMemo, useState } from 'react';
import {
  reportePaquetesPorArea,
  reporteTopFacturacion,
} from '../../../shared/api/endpoints';
import type { ListParams } from '../../../shared/api/params';
import { useAreas } from '../../../shared/hooks/useAreas';
import type { ConsultorFacturacion, PaquetePorArea } from '../../../shared/types';

export type TabKey = 'paquetesPorArea' | 'topFacturacion';

export interface ChartPoint {
  name: string;
  value: number;
}

/** Tablas, grafico y filtros de la pagina de reportes. */
export function useReportes() {
  const areas = useAreas();
  const [tab, setTabState] = useState<TabKey>('paquetesPorArea');
  const [area, setAreaState] = useState('');
  const [activo, setActivoState] = useState('');

  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortBy, setSortBy] = useState('Area');
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>('asc');

  const [paquetes, setPaquetes] = useState<PaquetePorArea[]>([]);
  const [facturacion, setFacturacion] = useState<ConsultorFacturacion[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [chartData, setChartData] = useState<ChartPoint[]>([]);
  const [chartLoading, setChartLoading] = useState(true);

  const setTab = useCallback((next: TabKey) => {
    setTabState(next);
    setPage(0);
    setSortBy(next === 'paquetesPorArea' ? 'Area' : 'FacturacionEstimada');
    setSortDir('asc');
  }, []);
  const setArea = useCallback((value: string) => {
    setAreaState(value);
    setPage(0);
  }, []);
  const setActivo = useCallback((value: string) => {
    setActivoState(value);
    setPage(0);
  }, []);

  const selectedArea = area || undefined;
  const active = activo === '' ? undefined : activo === 'active';

  const tableParams = useMemo<ListParams>(
    () => ({ page: page + 1, pageSize, sortBy, sortDir, area: selectedArea, activo: active }),
    [page, pageSize, sortBy, sortDir, selectedArea, active],
  );

  const chartParams = useMemo<ListParams>(
    () => ({
      page: 1,
      pageSize: 100,
      sortBy: tab === 'paquetesPorArea' ? 'TotalMonto' : 'FacturacionEstimada',
      sortDir: 'desc',
      area: selectedArea,
      activo: active,
    }),
    [tab, selectedArea, active],
  );

  useEffect(() => {
    let cancelled = false;
    const run = async () => {
      setLoading(true);
      setError('');
      try {
        if (tab === 'paquetesPorArea') {
          const result = await reportePaquetesPorArea(tableParams);
          if (cancelled) return;
          setPaquetes(result.items);
          setTotalCount(result.totalCount);
        } else {
          const result = await reporteTopFacturacion(tableParams);
          if (cancelled) return;
          setFacturacion(result.items);
          setTotalCount(result.totalCount);
        }
      } catch {
        if (!cancelled) setError('No se pudo cargar el reporte. Revisa la conexión con la API.');
      } finally {
        if (!cancelled) setLoading(false);
      }
    };
    void run();
    return () => {
      cancelled = true;
    };
  }, [tab, tableParams]);

  useEffect(() => {
    // Grafico: pide hasta 100 filas con el mismo filtro para visualizar
    // la distribucion completa (independiente de la paginacion de la tabla).
    let cancelled = false;
    const load = async () => {
      setChartLoading(true);
      try {
        if (tab === 'paquetesPorArea') {
          const items = (await reportePaquetesPorArea(chartParams)).items;
          if (cancelled) return;
          setChartData(items.map((r) => ({ name: r.area, value: r.totalMonto })));
        } else {
          const items = (await reporteTopFacturacion(chartParams)).items;
          if (cancelled) return;
          setChartData(items.map((r) => ({ name: r.nombreCompleto, value: r.facturacionEstimada })));
        }
      } catch {
        if (!cancelled) setChartData([]);
      } finally {
        if (!cancelled) setChartLoading(false);
      }
    };
    void load();
    return () => {
      cancelled = true;
    };
  }, [tab, chartParams]);

  const handleSort = useCallback(
    (column: string) => {
      if (sortBy === column) {
        setSortDir((current) => (current === 'asc' ? 'desc' : 'asc'));
      } else {
        setSortBy(column);
        setSortDir('asc');
      }
    },
    [sortBy],
  );

  return {
    tab,
    setTab,
    area,
    setArea,
    activo,
    setActivo,
    areas,
    paquetes,
    facturacion,
    totalCount,
    loading,
    error,
    page,
    setPage,
    pageSize,
    setPageSize,
    sortBy,
    sortDir,
    handleSort,
    chartData,
    chartLoading,
  };
}