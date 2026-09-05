import { useState } from 'react';
import {
  Alert,
  Box,
  Button,
  IconButton,
  Paper,
  TextField,
  Toolbar,
  Typography,
} from '@mui/material';
import { Add, Delete, Edit } from '@mui/icons-material';
import ConfirmDialog from '../../../shared/ui/ConfirmDialog';
import DataTable, { type Column } from '../../../shared/ui/DataTable';
import { formatCurrency } from '../../../shared/utils/format';
import { useAreas } from '../../../shared/hooks/useAreas';
import { getApiErrorMessage } from '../../../shared/api/errors';
import { useAuth } from '../../../app/hooks/useAuth';
import { useToast } from '../../../app/hooks/useToast';
import PaqueteForm from '../components/PaqueteForm';
import { usePaquetes } from '../hooks/usePaquetes';
import type { Paquete, PaquetePayload } from '../../../shared/types';

export default function PaquetesPage() {
  const { isAdmin } = useAuth();
  const { showToast } = useToast();
  const areas = useAreas();
  const {
    rows,
    totalCount,
    page,
    pageSize,
    sortBy,
    sortDir,
    loading,
    error,
    filters,
    setFilter,
    setPage,
    setPageSize,
    handleSort,
    reload,
    create,
    update,
    remove,
  } = usePaquetes();

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<Paquete | null>(null);
  const [deleting, setDeleting] = useState<Paquete | null>(null);

  const handleSave = async (payload: PaquetePayload) => {
    try {
      if (editing) {
        await update(editing.id, payload);
        showToast('Paquete actualizado correctamente.');
      } else {
        await create(payload);
        showToast('Paquete creado correctamente.');
      }
      reload();
    } catch (error) {
      showToast(getApiErrorMessage(error), 'error');
      throw error;
    }
  };

  const handleDelete = async () => {
    if (!deleting) return;
    try {
      await remove(deleting.id);
      showToast('Paquete eliminado correctamente.');
      reload();
    } catch (error) {
      showToast(getApiErrorMessage(error), 'error');
    } finally {
      setDeleting(null);
    }
  };

  const openNewForm = () => {
    setEditing(null);
    setFormOpen(true);
  };

  const openEditForm = (paquete: Paquete) => {
    setEditing(paquete);
    setFormOpen(true);
  };

  const columns: Column<Paquete>[] = [
    { key: 'Nombre', valueKey: 'nombre', label: 'Nombre', sortable: true },
    { key: 'descripcion', valueKey: 'descripcion', label: 'Descripción', render: (row) => row.descripcion || '—' },
    { key: 'Area', valueKey: 'area', label: 'Area', sortable: true },
    {
      key: 'Precio',
      valueKey: 'precio',
      label: 'Precio',
      sortable: true,
      align: 'right',
      render: (row) => formatCurrency(row.precio),
    },
    { key: 'activo', valueKey: 'activo', label: 'Estado', render: (row) => (row.activo ? 'Activo' : 'Inactivo') },
  ];

  if (isAdmin) {
    columns.push({
      key: 'acciones',
      label: 'Acciones',
      align: 'right',
      render: (row) => (
        <>
          <IconButton aria-label={`Editar paquete ${row.nombre}`} onClick={() => openEditForm(row)}>
            <Edit />
          </IconButton>
          <IconButton color="error" aria-label={`Eliminar paquete ${row.nombre}`} onClick={() => setDeleting(row)}>
            <Delete />
          </IconButton>
        </>
      ),
    });
  }

  return (
    <Box>
      <Toolbar sx={{ px: 0, justifyContent: 'space-between' }}>
        <Typography variant="h5">Paquetes de servicio</Typography>
        {isAdmin && (
          <Button variant="contained" startIcon={<Add />} onClick={openNewForm}>
            Nuevo paquete
          </Button>
        )}
      </Toolbar>

      <Paper sx={{ p: 2, mb: 2 }}>
        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
          <TextField
            label="Buscar por nombre"
            value={filters.nombre}
            onChange={(e) => setFilter({ nombre: e.target.value })}
            size="small"
            sx={{ minWidth: 240 }}
          />
          <TextField
            label="Área"
            select
            value={filters.area}
            onChange={(e) => setFilter({ area: e.target.value })}
            size="small"
            sx={{ minWidth: 180 }}
            slotProps={{ select: { native: true }, inputLabel: { shrink: true } }}
          >
            <option value="">Todos</option>
            {areas.map((a) => (
              <option key={a} value={a}>
                {a}
              </option>
            ))}
          </TextField>
          <TextField
            label="Estado"
            select
            value={filters.activo}
            onChange={(e) => setFilter({ activo: e.target.value })}
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
        emptyText="No hay paquetes."
      />

      <PaqueteForm
        key={formOpen ? editing?.id ?? 'new' : 'closed'}
        open={formOpen}
        initial={editing}
        areas={areas}
        onSave={handleSave}
        onClose={() => setFormOpen(false)}
      />

      <ConfirmDialog
        open={Boolean(deleting)}
        title="Eliminar paquete"
        message={`Se desactivará el paquete "${deleting?.nombre}". ¿Continuar?`}
        onConfirm={handleDelete}
        onClose={() => setDeleting(null)}
      />
    </Box>
  );
}