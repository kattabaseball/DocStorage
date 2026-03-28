export interface Case {
  id: string;
  name: string;
  description: string;
  destination: string;
  departureDate: string;
  returnDate: string;
  status: string;
  memberCount: number;
  readinessPercent: number;
  checklistId: string;
  checklistName: string;
  createdAt: string;
  updatedAt: string;
}

export interface CaseDetail extends Case {
  members: CaseMember[];
  checklist: CaseChecklistItem[];
  auditTrail: CaseAuditEntry[];
}

export interface CaseMember {
  id: string;
  memberId: string;
  memberName: string;
  avatarUrl: string | null;
  readinessPercent: number;
  pendingDocs: number;
  totalDocs: number;
}

export interface CaseChecklistItem {
  id: string;
  name: string;
  category: string;
  required: boolean;
  completedCount: number;
  totalCount: number;
}

export interface CaseAuditEntry {
  id: string;
  action: string;
  description: string;
  performedBy: string;
  performedAt: string;
}

export interface CaseAccess {
  id: string;
  email: string;
  fullName: string;
  permission: string;
  grantedAt: string;
  expiresAt: string | null;
  isActive: boolean;
}

export interface GrantAccessRequest {
  email: string;
  permission: string;
  expiresAt?: string;
}
