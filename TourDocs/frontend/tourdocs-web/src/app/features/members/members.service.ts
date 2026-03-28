import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '@core/services/api.service';
import { ApiResponse, PaginatedResult } from '@core/models/api-response.model';
import { QueryParams } from '@core/models/pagination.model';
import { Member, MemberDetail } from './members.models';

@Injectable({ providedIn: 'root' })
export class MembersService {
  private readonly api = inject(ApiService);
  private readonly path = 'members';

  getMembers(params: QueryParams): Observable<ApiResponse<PaginatedResult<Member>>> {
    return this.api.getPaginated<Member>(this.path, params);
  }

  getMember(id: string): Observable<ApiResponse<MemberDetail>> {
    return this.api.get<MemberDetail>(`${this.path}/${id}`);
  }

  createMember(member: Partial<MemberDetail>): Observable<ApiResponse<Member>> {
    return this.api.post<Member>(this.path, member);
  }

  updateMember(id: string, member: Partial<MemberDetail>): Observable<ApiResponse<Member>> {
    return this.api.put<Member>(`${this.path}/${id}`, member);
  }

  deleteMember(id: string): Observable<ApiResponse<void>> {
    return this.api.delete<void>(`${this.path}/${id}`);
  }

  inviteMember(email: string, role: string): Observable<ApiResponse<void>> {
    return this.api.post<void>(`${this.path}/invite`, { email, role });
  }
}
