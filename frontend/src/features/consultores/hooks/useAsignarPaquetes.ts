import { useEffect, useState } from 'react';
import { useToast } from '../../../app/hooks/useToast';
import { getApiErrorMessage } from '../../../shared/api/errors';
import {
  assignPaqueteToConsultor,
  getPaquete,
  listPaquetes,
  listPaquetesByConsultor,
  unassignPaqueteFromConsultor,
} from '../../../shared/api/endpoints';
import type { Consultor, ConsultorPaquete, Paquete } from '../../../shared/types';

export const MAX_PAQUETES = 5;

/** Estado del dialogo de asignacion de paquetes a un consultor. */
export function useAsignarPaquetes(open: boolean, consultor: Consultor | null) {
  const { showToast } = useToast();
  const [assigned, setAssigned] = useState<ConsultorPaquete[]>([]);
  const [available, setAvailable] = useState<Paquete[]>([]);
  const [selected, setSelected] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!open || !consultor) return;

    let cancelled = false;
    const load = async () => {
      setLoading(true);
      try {
        const [assignment, pool] = await Promise.all([
          listPaquetesByConsultor(consultor.id),
          listPaquetes({ page: 1, pageSize: 100, activo: true }),
        ]);
        if (cancelled) return;
        const assignedIds = new Set(assignment.map((a) => a.paqueteId));
        setAssigned(assignment);
        setAvailable(pool.items.filter((p) => !assignedIds.has(p.id)));
        setSelected('');
      } catch (error) {
        if (!cancelled) showToast(getApiErrorMessage(error), 'error');
      } finally {
        if (!cancelled) setLoading(false);
      }
    };
    void load();
    return () => {
      cancelled = true;
    };
  }, [open, consultor, showToast]);

  const assign = async () => {
    if (!consultor || !selected) return;
    const paqueteId = Number(selected);
    try {
      const result = await assignPaqueteToConsultor(consultor.id, paqueteId);
      setAssigned(result);
      setAvailable((current) => current.filter((p) => p.id !== paqueteId));
      setSelected('');
      showToast('Paquete asignado correctamente.');
    } catch (error) {
      showToast(getApiErrorMessage(error), 'error');
    }
  };

  const unassign = async (paqueteId: number) => {
    if (!consultor) return;
    try {
      const [result, paquete] = await Promise.all([
        unassignPaqueteFromConsultor(consultor.id, paqueteId),
        getPaquete(paqueteId),
      ]);
      setAssigned(result);
      setAvailable((current) => [...current, paquete]);
      showToast('Paquete desasignado correctamente.');
    } catch (error) {
      showToast(getApiErrorMessage(error), 'error');
    }
  };

  return { assigned, available, selected, setSelected, loading, assign, unassign };
}