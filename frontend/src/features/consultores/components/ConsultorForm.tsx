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
import type { Consultor, ConsultorPayload } from '../../../shared/types';

interface ConsultorFormProps {
  open: boolean;
  initial?: Consultor | null;
  areas: string[];
  onSave: (payload: ConsultorPayload) => Promise<void>;
  onClose: () => void;
}

const empty: ConsultorPayload = {
  nombreCompleto: '',
  email: '',
  area: '',
  tarifaHora: 0,
  activo: true,
  proyectosActivos: 0,
};

export default function ConsultorForm({ open, initial, areas, onSave, onClose }: ConsultorFormProps) {
  const [form, setForm] = useState<ConsultorPayload>(() => (initial ? { ...initial } : empty));
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [saving, setSaving] = useState(false);

  const validate = () => {
    const next: Record<string, string> = {};
    if (!form.nombreCompleto.trim()) next.nombreCompleto = 'El nombre completo es obligatorio.';
    if (!form.email.trim()) {
      next.email = 'El correo es obligatorio.';
    } else if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(form.email.trim())) {
      next.email = 'Introduce un correo electrónico válido.';
    }
    if (!form.area.trim()) next.area = 'El área es obligatoria.';
    if (form.tarifaHora < 30 || form.tarifaHora > 200) {
      next.tarifaHora = 'La tarifa/hora debe estar entre 30 y 200.';
    }
    if (form.proyectosActivos < 0 || form.proyectosActivos > 5) {
      next.proyectosActivos = 'Los proyectos activos deben estar entre 0 y 5.';
    }
    setErrors(next);
    return Object.keys(next).length === 0;
  };

  const handleSubmit = async () => {
    if (!validate()) return;
    setSaving(true);
    try {
      await onSave({
        ...form,
        nombreCompleto: form.nombreCompleto.trim(),
        email: form.email.trim(),
        area: form.area.trim(),
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
      <DialogTitle>{initial ? 'Editar consultor' : 'Nuevo consultor'}</DialogTitle>
      <DialogContent>
        <TextField
          label="Nombre completo"
          value={form.nombreCompleto}
          onChange={(e) => setForm({ ...form, nombreCompleto: e.target.value })}
          error={Boolean(errors.nombreCompleto)}
          helperText={errors.nombreCompleto}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Correo"
          type="email"
          value={form.email}
          onChange={(e) => setForm({ ...form, email: e.target.value })}
          error={Boolean(errors.email)}
          helperText={errors.email}
          fullWidth
          margin="normal"
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
          label="Tarifa/hora (USD)"
          type="number"
          slotProps={{ htmlInput: { min: 30, max: 200, step: '0.01' } }}
          value={form.tarifaHora}
          onChange={(e) => setForm({ ...form, tarifaHora: Number(e.target.value) })}
          error={Boolean(errors.tarifaHora)}
          helperText={errors.tarifaHora || 'Debe estar entre 30 y 200.'}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Proyectos activos"
          type="number"
          slotProps={{ htmlInput: { min: 0, max: 5 } }}
          value={form.proyectosActivos}
          onChange={(e) => setForm({ ...form, proyectosActivos: Number(e.target.value) })}
          error={Boolean(errors.proyectosActivos)}
          helperText={errors.proyectosActivos || 'Debe estar entre 0 y 5.'}
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