import { useState } from 'react';
import {
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  TextField,
} from '@mui/material';
import type { Paquete, PaquetePayload } from '../../../shared/types';

interface PaqueteFormProps {
  open: boolean;
  initial?: Paquete | null;
  areas: string[];
  onSave: (payload: PaquetePayload) => Promise<void>;
  onClose: () => void;
}

const empty: PaquetePayload = {
  nombre: '',
  descripcion: '',
  area: '',
  precio: 0,
  activo: true,
};

export default function PaqueteForm({ open, initial, areas, onSave, onClose }: PaqueteFormProps) {
  const [form, setForm] = useState<PaquetePayload>(() => (initial ? { ...initial } : empty));
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [saving, setSaving] = useState(false);

  const validate = () => {
    const next: Record<string, string> = {};
    if (!form.nombre.trim()) next.nombre = 'El nombre es obligatorio.';
    if (!form.area.trim()) next.area = 'El área es obligatoria.';
    if (form.precio < 0) next.precio = 'El precio debe ser mayor o igual a 0.';
    setErrors(next);
    return Object.keys(next).length === 0;
  };

  const handleSubmit = async () => {
    if (!validate()) return;
    setSaving(true);
    try {
      await onSave({
        ...form,
        nombre: form.nombre.trim(),
        area: form.area.trim(),
        descripcion: form.descripcion?.trim() || undefined,
      });
      onClose();
    } catch {
      // El error ya se notifica via toast; se mantiene el dialogo abierto.
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>{initial ? 'Editar paquete' : 'Nuevo paquete'}</DialogTitle>
      <DialogContent>
        <TextField
          label="Nombre"
          value={form.nombre}
          onChange={(e) => setForm({ ...form, nombre: e.target.value })}
          error={Boolean(errors.nombre)}
          helperText={errors.nombre}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Descripción"
          value={form.descripcion}
          onChange={(e) => setForm({ ...form, descripcion: e.target.value })}
          fullWidth
          margin="normal"
          multiline
          minRows={2}
        />
        {areas.length > 0 ? (
          <TextField
            label="Área"
            select
            value={form.area}
            onChange={(e) => setForm({ ...form, area: e.target.value })}
            error={Boolean(errors.area)}
            helperText={errors.area}
            fullWidth
            margin="normal"
            slotProps={{ select: { native: true }, inputLabel: { shrink: true } }}
          >
            {!form.area && <option value="">Selecciona un área</option>}
            {areas.map((a) => (
              <option key={a} value={a}>
                {a}
              </option>
            ))}
          </TextField>
        ) : (
          <TextField
            label="Área"
            value={form.area}
            onChange={(e) => setForm({ ...form, area: e.target.value })}
            error={Boolean(errors.area)}
            helperText={errors.area}
            fullWidth
            margin="normal"
          />
        )}
        <TextField
          label="Precio"
          type="number"
          slotProps={{ htmlInput: { min: 0, step: '0.01' } }}
          value={form.precio}
          onChange={(e) => setForm({ ...form, precio: Number(e.target.value) })}
          error={Boolean(errors.precio)}
          helperText={errors.precio}
          fullWidth
          margin="normal"
        />
        <FormControlLabel
          control={
            <Checkbox
              checked={form.activo}
              onChange={(e) => setForm({ ...form, activo: e.target.checked })}
            />
          }
          label="Activo"
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color="inherit">
          Cancelar
        </Button>
        <Button onClick={handleSubmit} variant="contained" disabled={saving}>
          Guardar
        </Button>
      </DialogActions>
    </Dialog>
  );
}