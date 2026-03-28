import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { ApiResponse, PaginatedResult } from '../models/api-response.model';
import { QueryParams } from '../models/pagination.model';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  get<T>(path: string, params?: Record<string, string | number | boolean>): Observable<ApiResponse<T>> {
    let httpParams = new HttpParams();
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        httpParams = httpParams.set(key, String(value));
      });
    }
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${path}`, { params: httpParams });
  }

  getPaginated<T>(path: string, queryParams: QueryParams): Observable<ApiResponse<PaginatedResult<T>>> {
    let httpParams = new HttpParams()
      .set('pageNumber', String(queryParams.pageNumber))
      .set('pageSize', String(queryParams.pageSize));

    if (queryParams.sortBy) {
      httpParams = httpParams.set('sortBy', queryParams.sortBy);
    }
    if (queryParams.sortDirection) {
      httpParams = httpParams.set('sortDirection', queryParams.sortDirection);
    }
    if (queryParams.search) {
      httpParams = httpParams.set('search', queryParams.search);
    }
    if (queryParams.filters) {
      queryParams.filters.forEach((filter, index) => {
        httpParams = httpParams
          .set(`filters[${index}].field`, filter.field)
          .set(`filters[${index}].operator`, filter.operator)
          .set(`filters[${index}].value`, filter.value);
      });
    }

    return this.http.get<ApiResponse<PaginatedResult<T>>>(`${this.baseUrl}/${path}`, { params: httpParams });
  }

  post<T>(path: string, body: unknown): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(`${this.baseUrl}/${path}`, body);
  }

  put<T>(path: string, body: unknown): Observable<ApiResponse<T>> {
    return this.http.put<ApiResponse<T>>(`${this.baseUrl}/${path}`, body);
  }

  delete<T>(path: string): Observable<ApiResponse<T>> {
    return this.http.delete<ApiResponse<T>>(`${this.baseUrl}/${path}`);
  }

  upload<T>(path: string, file: File, additionalData?: Record<string, string>): Observable<ApiResponse<T>> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    if (additionalData) {
      Object.entries(additionalData).forEach(([key, value]) => {
        formData.append(key, value);
      });
    }
    return this.http.post<ApiResponse<T>>(`${this.baseUrl}/${path}`, formData);
  }

  download(path: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${path}`, {
      responseType: 'blob'
    });
  }
}
