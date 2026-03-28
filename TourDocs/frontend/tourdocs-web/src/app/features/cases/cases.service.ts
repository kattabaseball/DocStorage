import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '@core/services/api.service';
import { ApiResponse, PaginatedResult } from '@core/models/api-response.model';
import { QueryParams } from '@core/models/pagination.model';
import { Case, CaseDetail, CaseAccess, GrantAccessRequest } from './cases.models';

@Injectable({ providedIn: 'root' })
export class CasesService {
  private readonly api = inject(ApiService);
  private readonly path = 'cases';

  getCases(params: QueryParams): Observable<ApiResponse<PaginatedResult<Case>>> {
    return this.api.getPaginated<Case>(this.path, params);
  }

  getCase(id: string): Observable<ApiResponse<CaseDetail>> {
    return this.api.get<CaseDetail>(`${this.path}/${id}`);
  }

  createCase(data: Partial<Case>): Observable<ApiResponse<Case>> {
    return this.api.post<Case>(this.path, data);
  }

  updateCase(id: string, data: Partial<Case>): Observable<ApiResponse<Case>> {
    return this.api.put<Case>(`${this.path}/${id}`, data);
  }

  deleteCase(id: string): Observable<ApiResponse<void>> {
    return this.api.delete<void>(`${this.path}/${id}`);
  }

  addMember(caseId: string, memberId: string): Observable<ApiResponse<void>> {
    return this.api.post<void>(`${this.path}/${caseId}/members`, { memberId });
  }

  removeMember(caseId: string, memberId: string): Observable<ApiResponse<void>> {
    return this.api.delete<void>(`${this.path}/${caseId}/members/${memberId}`);
  }

  grantAccess(caseId: string, request: GrantAccessRequest): Observable<ApiResponse<CaseAccess>> {
    return this.api.post<CaseAccess>(`${this.path}/${caseId}/access`, request);
  }

  revokeAccess(accessId: string): Observable<ApiResponse<void>> {
    return this.api.delete<void>(`${this.path}/access/${accessId}`);
  }
}
