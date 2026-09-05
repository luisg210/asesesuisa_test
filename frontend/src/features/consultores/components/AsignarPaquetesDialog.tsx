import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  List,
  ListItem,
  ListItemText,
  TextField,
  Typography,
} from '@mui/material';
import { Delete } from '@mui/icons-material';
import { useAsignarPaquetes, MAX_PAQUETES } from '../hooks/useAsignarPaquetes';
import { formatCurrency } from '../../../shared/utils/format';
import type { Consultor } from '../../../shared/types';

interface AsignarPaquetesDialogProps {
  open: boolean;
  consultor: Consultor | null;
  onClose: () => void;
}

export default function AsignarPaquetesDialog({ open, consultor, onClose }: AsignarPaquetesDialogProps) {
  const { assigned, available, selected, setSelected, loading, assign, unassign } = useAsignarPaquetes(
    open,
    consultor,
  );

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Asignar paquetes a {consultor?.nombreCompleto}</DialogTitle>
      <DialogContent dividers>
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
            <CircularProgress />
          </Box>
        ) : (
          <>
            <Box sx={{ display: 'flex', gap: 1, mb: 2, alignItems: 'center' }}>
              <TextField
                label="Paquete disponible"
                select
                value={selected}
                onChange={(e) => setSelected(e.target.value)}
                size="small"
                fullWidth
                disabled={assigned.length >= MAX_PAQUETES}
                slotProps={{ select: { native: true }, inputLabel: { shrink: true } }}
              >
                <option value="">Seleccionar…</option>
                {available.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.nombre} — {p.area} ({formatCurrency(p.precio)})
                  </option>
                ))}
              </TextField>
              <Button
                variant="contained"
                onClick={() => void assign()}
                disabled={!selected || assigned.length >= MAX_PAQUETES}
                sx={{ minWidth: 120 }}
              >
                Asignar
              </Button>
            </Box>

            {assigned.length >= MAX_PAQUETES && (
              <Typography variant="body2" color="warning.main" sx={{ mb: 1 }}>
                El consultor ya tiene el máximo de {MAX_PAQUETES} paquetes asignados.
              </Typography>
            )}

            <Typography variant="subtitle2" sx={{ mb: 1 }}>
              Asignados ({assigned.length}/{MAX_PAQUETES})
            </Typography>
            {assigned.length === 0 ? (
              <Typography variant="body2" color="text.secondary">
                Este consultor no tiene paquetes asignados.
              </Typography>
            ) : (
              <List dense disablePadding>
                {assigned.map((item) => (
                  <ListItem
                    key={item.paqueteId}
                    secondaryAction={
                      <IconButton
                        edge="end"
                        color="error"
                        aria-label={`Quitar ${item.nombre}`}
                        onClick={() => void unassign(item.paqueteId)}
                      >
                        <Delete />
                      </IconButton>
                    }
                  >
                    <ListItemText
                      primary={item.nombre}
                      secondary={`${item.area} — ${formatCurrency(item.precio)}`}
                    />
                  </ListItem>
                ))}
              </List>
            )}
          </>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cerrar</Button>
      </DialogActions>
    </Dialog>
  );
}