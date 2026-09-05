import {
  Alert,
  Box,
  Paper,
  Tabs,
  Tab,
  TextField,
  Typography,
} from '@mui/material';
import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import DataTable, { type Column } from '../../../shared/ui/DataTable';
import { formatCurrency } from '../../../shared/utils/format';
import { useReportes } from '../hooks/useReportes';
import type { ConsultorFacturacion, PaquetePorArea } from '../../../shared/types';

export default function ReportesPage() {
  const {
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
  } = useReportes();

  const tableProps = {
    loading,
    emptyText: 'No hay datos.',
    totalCount,
    page,
    pageSize,
    onPageChange: setPage,
    onRowsPerPageChange: setPageSize,
    sortBy,
    sortDir,
    onSort: handleSort,
  };

  const chartSeriesName = tab === 'paquetesPorArea' ? 'Monto total' : 'Facturación estimada';

  const paquetesColumns: Column<PaquetePorArea>[] = [
    { key: 'Area', valueKey: 'area', label: 'Área', sortable: true },
    { key: 'TotalPaquetes', valueKey: 'totalPaquetes', label: 'Total paquetes', sortable: true, align: 'right' },
    { key: 'TotalMonto', valueKey: 'totalMonto', label: 'Total monto', sortable: true, align: 'right', render: (row) => formatCurrency(row.totalMonto) },
    { key: 'PrecioMinimo', valueKey: 'precioMinimo', label: 'Precio mínimo', align: 'right', render: (row) => formatCurrency(row.precioMinimo) },
    { key: 'PrecioMaximo', valueKey: 'precioMaximo', label: 'Precio máximo', align: 'right', render: (row) => formatCurrency(row.precioMaximo) },
  ];

  const facturacionColumns: Column<ConsultorFacturacion>[] = [
    { key: 'NombreCompleto', valueKey: 'nombreCompleto', label: 'Nombre completo', sortable: true },
    { key: 'Area', valueKey: 'area', label: 'Área', sortable: true },
    { key: 'TarifaHora', valueKey: 'tarifaHora', label: 'Tarifa/hora', sortable: true, align: 'right', render: (row) => formatCurrency(row.tarifaHora) },
    { key: 'ProyectosActivos', valueKey: 'proyectosActivos', label: 'Proyectos', sortable: true, align: 'right' },
    { key: 'FacturacionEstimada', valueKey: 'facturacionEstimada', label: 'Facturación estimada', sortable: true, align: 'right', render: (row) => formatCurrency(row.facturacionEstimada) },
  ];

  return (
    <Box>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Reportes
      </Typography>

      <Paper sx={{ mb: 2 }}>
        <Tabs value={tab} onChange={(_, v: 'paquetesPorArea' | 'topFacturacion') => { setTab(v); }}>
          <Tab label="Paquetes por área" value="paquetesPorArea" />
          <Tab label="Consultores top facturación" value="topFacturacion" />
        </Tabs>
        <Box sx={{ p: 2, display: 'flex', gap: 2 }}>
          <TextField
            label="Área"
            select
            value={area}
            onChange={(e) => { setArea(e.target.value); setPage(0); }}
            size="small"
            sx={{ minWidth: 180 }}
            slotProps={{ select: { native: true }, inputLabel: { shrink: true } }}
          >
            <option value="">Todos</option>
            {areas.map((a) => (
              <option key={a} value={a}>{a}</option>
            ))}
          </TextField>
          <TextField
            label="Estado"
            select
            value={activo}
            onChange={(e) => { setActivo(e.target.value); setPage(0); }}
            size="small"
            sx={{ minWidth: 160 }}
            slotProps={{ select: { native: true }, inputLabel: { shrink: true } }}
          >
            <option value="">Todos</option>
            <option value="active">Activo</option>
            <option value="inactive">Inactivo</option>
          </TextField>
        </Box>
      </Paper>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {!chartLoading && chartData.length > 0 && (
        <Paper sx={{ mb: 2 }}>
          <Box sx={{ p: 2 }}>
            <Typography variant="h6" sx={{ mb: 2 }}>
              {tab === 'paquetesPorArea' ? 'Monto total por área' : 'Facturación estimada por consultor'}
            </Typography>
            <ResponsiveContainer width="100%" height={280}>
              <BarChart data={chartData} margin={{ top: 8, right: 16, left: 8, bottom: 8 }}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" tick={{ fontSize: 12 }} interval={0} angle={-20} textAnchor="end" height={80} />
                <YAxis tickFormatter={(v) => formatCurrency(Number(v))} width={95} />
                <Tooltip formatter={(value) => formatCurrency(Number(value))} />
                <Bar dataKey="value" name={chartSeriesName} fill="#1976d2" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </Box>
        </Paper>
      )}

      {tab === 'paquetesPorArea' ? (
        <DataTable
          columns={paquetesColumns}
          rows={paquetes}
          rowKey={(row) => row.area}
          {...tableProps}
        />
      ) : (
        <DataTable
          columns={facturacionColumns}
          rows={facturacion}
          rowKey={(row) => row.id}
          {...tableProps}
        />
      )}

      <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
        Facturación estimada = Tarifa/hora x 160 horas/mes x Proyectos activos.
      </Typography>
    </Box>
  );
}