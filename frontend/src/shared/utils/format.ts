const currencyFormatter = new Intl.NumberFormat('es-ES', { style: 'currency', currency: 'USD' });

export function formatCurrency(value: number): string {
  return currencyFormatter.format(value);
}