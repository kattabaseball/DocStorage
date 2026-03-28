import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '@core/services/api.service';
import { ApiResponse, PaginatedResult } from '@core/models/api-response.model';
import { QueryParams } from '@core/models/pagination.model';
import { Document, DocumentCategory, DocumentVerifyRequest } from './documents.models';

@Injectable({ providedIn: 'root' })
export class DocumentsService {
  private readonly api = inject(ApiService);
  private readonly path = 'documents';

  getDocuments(params: QueryParams): Observable<ApiResponse<PaginatedResult<Document>>> {
    return this.api.getPaginated<Document>(this.path, params);
  }

  getDocument(id: string): Observable<ApiResponse<Document>> {
    return this.api.get<Document>(`${this.path}/${id}`);
  }

  getCategories(): Observable<ApiResponse<DocumentCategory[]>> {
    return this.api.get<DocumentCategory[]>(`${this.path}/categories`);
  }

  verifyDocument(request: DocumentVerifyRequest): Observable<ApiResponse<Document>> {
    return this.api.post<Document>(`${this.path}/${request.documentId}/verify`, request);
  }

  deleteDocument(id: string): Observable<ApiResponse<void>> {
    return this.api.delete<void>(`${this.path}/${id}`);
  }

  getExpiringDocuments(daysAhead: number): Observable<ApiResponse<Document[]>> {
    return this.api.get<Document[]>(`${this.path}/expiring`, { daysAhead });
  }

  downloadDocument(id: string): Observable<Blob> {
    return this.api.download(`${this.path}/${id}/download`);
  }
}
