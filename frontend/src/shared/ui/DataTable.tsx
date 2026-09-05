import type { ReactNode } from 'react';
import {
  CircularProgress,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TableSortLabel,
} from '@mui/material';

export type CellAlign = 'inherit' | 'left' | 'center' | 'right' | 'justify';

export interface Column<T> {
  key: string;
  valueKey?: keyof T;
  label: string;
  sortable?: boolean;
  align?: CellAlign;
  render?: (row: T) => ReactNode;
}

export interface DataTableProps<T> {
  columns: Column<T>[];
  rows: T[];
  rowKey: (row: T) => string | number;
  loading?: boolean;
  emptyText?: string;
  totalCount: number;
  page: number;
  pageSize: number;
  onPageChange: (page: number) => void;
  onRowsPerPageChange: (pageSize: number) => void;
  sortBy?: string;
  sortDir?: 'asc' | 'desc';
  onSort?: (column: string) => void;
  rowsPerPageOptions?: number[];
}

function defaultCell<T>(row: T, column: Column<T>): ReactNode {
  const value = column.valueKey != null ? row[column.valueKey] : (row as Record<string, unknown>)[column.key];
  return value == null ? '' : String(value);
}

export default function DataTable<T>({
  columns,
  rows,
  rowKey,
  loading = false,
  emptyText = 'No hay datos.',
  totalCount,
  page,
  pageSize,
  onPageChange,
  onRowsPerPageChange,
  sortBy,
  sortDir,
  onSort,
  rowsPerPageOptions = [5, 10, 25],
}: DataTableProps<T>) {
  return (
    <Paper>
      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              {columns.map((column) => (
                <TableCell key={column.key} align={column.align}>
                  {column.sortable && onSort ? (
                    <TableSortLabel
                      active={sortBy === column.key}
                      direction={sortDir}
                      onClick={() => onSort(column.key)}
                    >
                      {column.label}
                    </TableSortLabel>
                  ) : (
                    column.label
                  )}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={columns.length} align="center">
                  <CircularProgress />
                </TableCell>
              </TableRow>
            ) : rows.length === 0 ? (
              <TableRow>
                <TableCell colSpan={columns.length} align="center">
                  {emptyText}
                </TableCell>
              </TableRow>
            ) : (
              rows.map((row) => (
                <TableRow key={rowKey(row)} hover>
                  {columns.map((column) => (
                    <TableCell key={column.key} align={column.align}>
                      {column.render ? column.render(row) : defaultCell(row, column)}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={totalCount}
        page={page}
        onPageChange={(_, newPage) => onPageChange(newPage)}
        rowsPerPage={pageSize}
        onRowsPerPageChange={(e) => onRowsPerPageChange(parseInt(e.target.value, 10))}
        rowsPerPageOptions={rowsPerPageOptions}
      />
    </Paper>
  );
}