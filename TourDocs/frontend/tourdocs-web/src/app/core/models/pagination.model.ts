export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
}

export interface SortParams {
  sortBy: string;
  sortDirection: 'asc' | 'desc';
}

export interface FilterParams {
  field: string;
  operator: 'eq' | 'neq' | 'gt' | 'gte' | 'lt' | 'lte' | 'contains' | 'startsWith' | 'endsWith';
  value: string;
}

export interface QueryParams extends PaginationParams {
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  search?: string;
  filters?: FilterParams[];
}

export const DEFAULT_PAGINATION: PaginationParams = {
  pageNumber: 1,
  pageSize: 25
};
