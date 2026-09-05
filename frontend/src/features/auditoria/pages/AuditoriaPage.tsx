import {
  Alert,
  Box,
  Paper,
  TextField,
  Typography,
} from '@mui/material';
import DataTable, { type Column } from '../../../shared/ui/DataTable';
import { useAuditoria } from '../hooks/useAuditoria';
import type { AuditoriaEntry } from '../../../shared/types';

export default function AuditoriaPage() {
  const {
    rows,
    totalCount,
    page,
    pageSize,
    setPage,
    setPageSize,
    sortBy,
    sortDir,
    handleSort,
    loading,
    error,
    filters,
    setFilter,
  } = useAuditoria();

  const formatDate = (value: string) => new Date(value).toLocaleString('es-ES');

  const columns: Column<AuditoriaEntry>[] = [
    { key: 'FechaHora', valueKey: 'fechaHora', label: 'Fecha', sortable: true, render: (row) => formatDate(row.fechaHora) },
    { key: 'Usuario', valueKey: 'usuario', label: 'Usuario', sortable: true },
    { key: 'Accion', valueKey: 'accion', label: 'Acción', sortable: true },
    { key: 'Entidad', valueKey: 'entidad', label: 'Entidad', sortable: true },
    { key: 'entidadId', valueKey: 'entidadId', label: 'Id', align: 'right', render: (row) => row.entidadId ?? '—' },
    { key: 'detalle', valueKey: 'detalle', label: 'Detalle', render: (row) => row.detalle ?? '—' },
    { key: 'ip', valueKey: 'ip', label: 'IP', render: (row) => row.ip ?? '—' },
  ];

  return (
    <Box>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Auditoría
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Bitácora de escrituras: creaciones, actualizaciones, eliminaciones, asignaciones e inicios de sesión.
      </Typography>

      <Paper sx={{ p: 2, mb: 2 }}>
        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
          <TextField
            label="Entidad"
            value={filters.entidad}
            onChange={(e) => setFilter({ entidad: e.target.value })}
            size="small"
            sx={{ minWidth: 180 }}
          />
          <TextField
            label="Acción"
            value={filters.accion}
            onChange={(e) => setFilter({ accion: e.target.value })}
            size="small"
            sx={{ minWidth: 140 }}
            helperText="CREATE, UPDATE, DELETE, ASSIGN, UNASSIGN, LOGIN"
          />
          <TextField
            label="Usuario"
            value={filters.usuario}
            onChange={(e) => setFilter({ usuario: e.target.value })}
            size="small"
            sx={{ minWidth: 200 }}
          />
        </Box>
      </Paper>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <DataTable
        columns={columns}
        rows={rows}
        rowKey={(row) => row.id}
        totalCount={totalCount}
        page={page}
        pageSize={pageSize}
        onPageChange={setPage}
        onRowsPerPageChange={setPageSize}
        sortBy={sortBy}
        sortDir={sortDir}
        onSort={handleSort}
        loading={loading}
        emptyText="Sin registros de auditoría."
      />
    </Box>
  );
}