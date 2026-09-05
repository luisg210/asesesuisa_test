import { useEffect, useState } from 'react';
import { listAreas } from '../api/endpoints';

/** Carga el catalogo de areas del backend; si falla queda vacio (texto libre). */
export function useAreas(): string[] {
  const [areas, setAreas] = useState<string[]>([]);

  useEffect(() => {
    let cancelled = false;
    listAreas()
      .then((result) => {
        if (!cancelled) setAreas(result);
      })
      .catch(() => {
        if (!cancelled) setAreas([]);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  return areas;
}