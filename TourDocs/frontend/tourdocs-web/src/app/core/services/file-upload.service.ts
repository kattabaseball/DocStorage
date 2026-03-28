import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpEventType, HttpEvent } from '@angular/common/http';
import { Observable, map, filter } from 'rxjs';
import { environment } from '@env/environment';

export interface UploadProgress {
  status: 'progress' | 'complete' | 'error';
  progress: number;
  body?: unknown;
  error?: string;
}

export interface DocumentUploadMetadata {
  memberId: string;
  documentTypeId: string;
  categoryId: string;
  expiryDate?: string;
  notes?: string;
}

@Injectable({ providedIn: 'root' })
export class FileUploadService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/documents`;

  uploadDocument(file: File, metadata: DocumentUploadMetadata): Observable<UploadProgress> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    formData.append('memberId', metadata.memberId);
    formData.append('documentTypeId', metadata.documentTypeId);
    formData.append('categoryId', metadata.categoryId);
    if (metadata.expiryDate) {
      formData.append('expiryDate', metadata.expiryDate);
    }
    if (metadata.notes) {
      formData.append('notes', metadata.notes);
    }

    return this.http.post(`${this.apiUrl}/upload`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map((event: HttpEvent<unknown>) => this.mapEventToProgress(event)),
      filter((progress): progress is UploadProgress => progress !== null)
    );
  }

  uploadMultiple(files: File[], metadata: DocumentUploadMetadata): Observable<UploadProgress> {
    const formData = new FormData();
    files.forEach((file, index) => {
      formData.append(`files`, file, file.name);
    });
    formData.append('memberId', metadata.memberId);
    formData.append('documentTypeId', metadata.documentTypeId);
    formData.append('categoryId', metadata.categoryId);
    if (metadata.expiryDate) {
      formData.append('expiryDate', metadata.expiryDate);
    }
    if (metadata.notes) {
      formData.append('notes', metadata.notes);
    }

    return this.http.post(`${this.apiUrl}/upload-multiple`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map((event: HttpEvent<unknown>) => this.mapEventToProgress(event)),
      filter((progress): progress is UploadProgress => progress !== null)
    );
  }

  private mapEventToProgress(event: HttpEvent<unknown>): UploadProgress | null {
    switch (event.type) {
      case HttpEventType.UploadProgress:
        return {
          status: 'progress',
          progress: event.total ? Math.round(100 * event.loaded / event.total) : 0
        };
      case HttpEventType.Response:
        return {
          status: 'complete',
          progress: 100,
          body: event.body
        };
      default:
        return null;
    }
  }
}
