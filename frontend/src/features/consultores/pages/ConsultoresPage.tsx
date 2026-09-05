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
import { Add, Delete, Edit, Link as LinkIcon } from '@mui/icons-material';
import ConfirmDialog from '../../../shared/ui/ConfirmDialog';
import DataTable, { type Column } from '../../../shared/ui/DataTable';
import { formatCurrency } from '../../../shared/utils/format';
import { useAreas } from '../../../shared/hooks/useAreas';
import { getApiErrorMessage } from '../../../shared/api/errors';
import { useAuth } from '../../../app/hooks/useAuth';
import { useToast } from '../../../app/hooks/useToast';
import ConsultorForm from '../components/ConsultorForm';
import AsignarPaquetesDialog from '../components/AsignarPaquetesDialog';
import { useConsultores } from '../hooks/useConsultores';
import type { Consultor, ConsultorPayload } from '../../../shared/types';

export default function ConsultoresPage() {
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
  } = useConsultores();

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<Consultor | null>(null);
  const [deleting, setDeleting] = useState<Consultor | null>(null);
  const [assigning, setAssigning] = useState<Consultor | null>(null);

  const handleSave = async (payload: ConsultorPayload) => {
    try {
      if (editing) {
        await update(editing.id, payload);
        showToast('Consultor actualizado correctamente.');
      } else {
        await create(payload);
        showToast('Consultor creado correctamente.');
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
      showToast('Consultor eliminado correctamente.');
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

  const openEditForm = (consultor: Consultor) => {
    setEditing(consultor);
    setFormOpen(true);
  };

  const columns: Column<Consultor>[] = [
    { key: 'NombreCompleto', valueKey: 'nombreCompleto', label: 'Nombre completo', sortable: true },
    { key: 'Email', valueKey: 'email', label: 'Correo', sortable: true },
    { key: 'Area', valueKey: 'area', label: 'Área', sortable: true },
    {
      key: 'TarifaHora',
      valueKey: 'tarifaHora',
      label: 'Tarifa/hora',
      sortable: true,
      align: 'right',
      render: (row) => formatCurrency(row.tarifaHora),
    },
    { key: 'ProyectosActivos', valueKey: 'proyectosActivos', label: 'Proyectos', sortable: true, align: 'right' },
    { key: 'activo', valueKey: 'activo', label: 'Estado', render: (row) => (row.activo ? 'Activo' : 'Inactivo') },
  ];

  if (isAdmin) {
    columns.push({
      key: 'acciones',
      label: 'Acciones',
      align: 'right',
      render: (row) => (
        <>
          <IconButton aria-label={`Asignar paquetes a ${row.nombreCompleto}`} onClick={() => setAssigning(row)}>
            <LinkIcon />
          </IconButton>
          <IconButton aria-label={`Editar consultor ${row.nombreCompleto}`} onClick={() => openEditForm(row)}>
            <Edit />
          </IconButton>
          <IconButton
            color="error"
            aria-label={`Eliminar consultor ${row.nombreCompleto}`}
            onClick={() => setDeleting(row)}
          >
            <Delete />
          </IconButton>
        </>
      ),
    });
  }

  return (
    <Box>
      <Toolbar sx={{ px: 0, justifyContent: 'space-between' }}>
        <Typography variant="h5">Consultores</Typography>
        {isAdmin && (
          <Button variant="contained" startIcon={<Add />} onClick={openNewForm}>
            Nuevo consultor
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
        emptyText="No hay consultores."
      />

      <ConsultorForm
        key={formOpen ? editing?.id ?? 'new' : 'closed'}
        open={formOpen}
        initial={editing}
        areas={areas}
        onSave={handleSave}
        onClose={() => setFormOpen(false)}
      />

      <AsignarPaquetesDialog
        open={Boolean(assigning)}
        consultor={assigning}
        onClose={() => setAssigning(null)}
      />

      <ConfirmDialog
        open={Boolean(deleting)}
        title="Eliminar consultor"
        message={`Se desactivará el consultor "${deleting?.nombreCompleto}". ¿Continuar?`}
        onConfirm={handleDelete}
        onClose={() => setDeleting(null)}
      />
    </Box>
  );
}