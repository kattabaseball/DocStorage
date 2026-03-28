export interface Document {
  id: string;
  title: string;
  documentType: string;
  category: string;
  status: string;
  memberId: string;
  memberName: string;
  fileUrl: string;
  fileName: string;
  fileSize: number;
  mimeType: string;
  expiryDate: string | null;
  uploadedAt: string;
  createdAt: string;
  updatedAt: string;
  verifiedAt: string | null;
  verificationNotes: string | null;
  isHardCopyNeeded: boolean;
  versionCount: number;
  currentVersionNumber: number;
  organizationId: string;
}

export interface DocumentCategory {
  id: string;
  name: string;
  description: string;
  documentsCount: number;
}

export interface DocumentUploadRequest {
  memberId: string;
  documentTypeId: string;
  categoryId: string;
  expiryDate?: string;
  notes?: string;
}

export interface DocumentVerifyRequest {
  documentId: string;
  approved: boolean;
  reason?: string;
}
